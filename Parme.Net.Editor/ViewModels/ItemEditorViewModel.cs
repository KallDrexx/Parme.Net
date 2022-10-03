using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;
using Parme.Net.Editor.ViewModels.ItemPropertyFields;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class ItemEditorViewModel : ObservableObject,
    IRecipient<ItemSelectedMessage>,
    IRecipient<TriggerSelectedMessage>,
    IRecipient<AddModifierMessage>,
    IRecipient<AddInitializerMessage>
{
    private readonly KnownBehaviors _knownBehaviors = new();
    private object? _currentObject;
    private Guid? _currentItemId;
    private bool _isUpdating;
    private bool _isNewModifier;
    private bool _isNewInitializer;
    
    [ObservableProperty] private Type? _selectedType;

    public ObservableCollection<ItemPropertyField> ItemProperties { get; } = new();
    public ObservableCollection<Type> FullTypeList { get; } = new();
    public bool HasItemSelected => _selectedType != null || _isNewModifier || _isNewInitializer;
    public bool HasProperties => ItemProperties.Any();

    public ItemEditorViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);

        ItemProperties.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasProperties));
        PropertyChanged += OnPropertyChanged;
    }

    public void Receive(ItemSelectedMessage message)
    {
        if (_isUpdating)
        {
            return;
        }
        
        _isUpdating = true;
        ClearFields();

        if (message.Value != null)
        {
            var modifier = WeakReferenceMessenger.Default.Send(new GetModifierDetailsRequest(message.Value.Value));
            if (modifier.Response != null)
            {
                UpdateFromModifier(modifier.Response);
            }
            else
            {
                var initializer = WeakReferenceMessenger.Default.Send(new GetInitializerDetailsRequest(message.Value.Value));
                if (initializer.Response != null)
                {
                    UpdateFromInitializer(initializer.Response);
                }
            }
        }

        NotifyReadOnlyPropertiesChanged();
        _isUpdating = false;
    }

    public void Receive(TriggerSelectedMessage message)
    {
        if (_isUpdating)
        {
            return;
        }
        
        _isUpdating = true;
        ClearFields();
        
        var trigger = WeakReferenceMessenger.Default.Send(new GetCurrentTriggerRequest());
        if (trigger.Response != null)
        {
            UpdateFromTrigger(trigger.Response);
        }
        
        NotifyReadOnlyPropertiesChanged();

        _isUpdating = false;
    }

    public void Receive(AddModifierMessage message)
    {
        if (_isUpdating)
        {
            return;
        }
        
        _isUpdating = true;
        ClearFields();

        _isNewModifier = true;
        _currentItemId = Guid.NewGuid();
        UpdateTypeList(_knownBehaviors.ModifierTypes, null);

        NotifyReadOnlyPropertiesChanged();
        _isUpdating = false;
    }

    public void Receive(AddInitializerMessage message)
    {
        if (_isUpdating)
        {
            return;
        }
        
        _isUpdating = true;
        ClearFields();

        _isNewInitializer = true;
        _currentItemId = Guid.NewGuid();
        UpdateTypeList(_knownBehaviors.InitializerTypes, null);

        NotifyReadOnlyPropertiesChanged();
        _isUpdating = false;
    }

    private void UpdateFromTrigger(ParticleTrigger trigger)
    {
        _currentObject = trigger;
        UpdateItemProperties(trigger, null);
        UpdateTypeList(_knownBehaviors.TriggerTypes, trigger.GetType());
    }

    private void UpdateFromModifier(TaggedModifier item)
    {
        _currentObject = item.Modifier;
        _currentItemId = item.Id;
        UpdateItemProperties(item.Modifier, item.Id);
        UpdateTypeList(_knownBehaviors.ModifierTypes, item.Modifier.GetType());
    }

    private void UpdateFromInitializer(TaggedInitializer item)
    {
        _currentObject = item.Initializer;
        _currentItemId = item.Id;
        UpdateItemProperties(item.Initializer, item.Id);
        UpdateTypeList(_knownBehaviors.InitializerTypes, item.Initializer.GetType());
    }

    private void UpdateItemProperties(object obj, Guid? itemId)
    {
        foreach (var property in ItemProperties)
        {
            property.Dispose();
        }
        
        ItemProperties.Clear();
        
        var selector = new ItemPropertyFieldSelector();
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.Name != "PropertiesIRead")
            .Where(x => x.Name != "PropertiesIUpdate")
            .Where(x => x.Name != "PropertiesISet")
            .Select(x => selector.GetItemProperty(obj, x, itemId))
            .ToArray();
        
        foreach (var property in properties)
        {
            ItemProperties.Add(property);
        }
    }

    private void UpdateTypeList(IReadOnlyList<Type> types, Type? selected)
    {
        FullTypeList.Clear();
        
        var selectedType = (Type?)null;
        foreach (var type in types)
        {
            FullTypeList.Add(type);
            if (type == selected)
            {
                selectedType = type;
            }
        }

        SelectedType = selectedType;
        OnPropertyChanged(nameof(HasItemSelected));
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        switch (e.PropertyName)
        {
            case nameof(SelectedType):
                SelectedTypeChanged();
                break;
        }
        _isUpdating = false;
    }

    private void SelectedTypeChanged()
    {
        if (SelectedType == null)
        {
            return;
        }

        if (_currentObject is ParticleTrigger)
        {
            _currentObject = _knownBehaviors.GetTriggerByType(SelectedType);
            UpdateFromTrigger((ParticleTrigger)_currentObject);
            WeakReferenceMessenger.Default.Send(new TriggerPropertyChangedMessage((ParticleTrigger)_currentObject));
        }
        
        else if (_currentObject is IParticleInitializer || _isNewInitializer)
        {
            _currentObject = _knownBehaviors.GetInitializerByType(SelectedType);
            var taggedInitializer = new TaggedInitializer(_currentItemId!.Value, (IParticleInitializer)_currentObject);

            UpdateFromInitializer(taggedInitializer);
            WeakReferenceMessenger.Default.Send(new InitializerPropertyChangedMessage(taggedInitializer));
        }
        
        else if (_currentObject is IParticleModifier || _isNewModifier)
        {
            _currentObject = _knownBehaviors.GetModifierByType(SelectedType);
            var taggedModifier = new TaggedModifier(_currentItemId!.Value, (IParticleModifier)_currentObject);
            
            UpdateFromModifier(taggedModifier);
            WeakReferenceMessenger.Default.Send(new ModifierPropertyChangedMessage(taggedModifier));
        }

        else
        {
            var message = $"Unknown behavior type of '{_currentObject?.GetType()}'";
            throw new InvalidOperationException(message);
        }

        WeakReferenceMessenger.Default.Send(new ItemSelectedMessage(_currentItemId));
    }

    private void ClearFields()
    {
        FullTypeList.Clear();
        SelectedType = null;
        ItemProperties.Clear();
        _currentObject = null;
        _currentItemId = null;
        _isNewModifier = false;
        _isNewInitializer = false;
    }

    private void NotifyReadOnlyPropertiesChanged()
    {
        OnPropertyChanged(nameof(HasProperties));
        OnPropertyChanged(nameof(HasItemSelected));
    }
}