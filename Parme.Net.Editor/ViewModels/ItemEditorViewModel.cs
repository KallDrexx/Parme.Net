using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;
using Parme.Net.Editor.ViewModels.ItemPropertyFields;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class ItemEditorViewModel : ObservableObject,
    IRecipient<ItemSelectedMessage>,
    IRecipient<TriggerSelectedMessage>
{
    private const string NoItemSelected = "<No Item Selected>";

    [ObservableProperty] private string _itemName = NoItemSelected;

    public bool HasNoProperties => _itemName != NoItemSelected && ItemProperties.Count == 0;
    public ObservableCollection<ItemPropertyField> ItemProperties { get; } = new();

    public ItemEditorViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);

        ItemProperties.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(HasNoProperties));
    }

    public void Receive(ItemSelectedMessage message)
    {
        ItemName = NoItemSelected;

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
    }

    public void Receive(TriggerSelectedMessage message)
    {
        ItemName = NoItemSelected;
        
        var trigger = WeakReferenceMessenger.Default.Send(new GetCurrentTriggerRequest());
        if (trigger.Response != null)
        {
            UpdateFromTrigger(trigger.Response);
        }
    }

    private void UpdateFromTrigger(ParticleTrigger trigger)
    {
        ItemName = trigger.GetType().Name;
        UpdateItemProperties(trigger, null);
    }

    private void UpdateFromModifier(TaggedModifier item)
    {
        ItemName = item.Modifier.GetType().Name;
        UpdateItemProperties(item.Modifier, item.Id);
    }

    private void UpdateFromInitializer(TaggedInitializer item)
    {
        ItemName = item.Initializer.GetType().Name;
        UpdateItemProperties(item.Initializer, item.Id);
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
}