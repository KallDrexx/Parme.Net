using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels.ItemPropertyFields;

public abstract class ItemPropertyField : ObservableObject, IDisposable
{
    private readonly object _backingInstance;
    private readonly PropertyInfo _propertyInfo;
    private readonly Guid? _itemId;
    private bool _isUpdating;
    private object? _rawValue;

    protected object? RawValue
    {
        get => _rawValue;
        set => SetProperty(ref _rawValue, value);
    }
    
    public string PropertyName { get; }

    protected ItemPropertyField(object instance, PropertyInfo property, Guid? itemId)
    {
        PropertyName = property.Name;
        _rawValue = property.GetValue(instance);

        _backingInstance = instance;
        _propertyInfo = property;
        _itemId = itemId;
        
        PropertyChanged += OnPropertyChanged;
        
        StrongReferenceMessenger.Default.RegisterAll(this);
    }

    public void Dispose()
    {
        StrongReferenceMessenger.Default.UnregisterAll(this);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        _propertyInfo.SetValue(_backingInstance, _rawValue);

        switch (_backingInstance)
        {
            case IParticleInitializer particleInitializer:
                WeakReferenceMessenger.Default.Send(
                    new InitializerPropertyChangedMessage(
                        new TaggedInitializer(_itemId!.Value, particleInitializer.Clone())));
                
                break;
            
            case IParticleModifier particleModifier:
                WeakReferenceMessenger.Default.Send(
                    new ModifierPropertyChangedMessage(
                        new TaggedModifier(_itemId!.Value, particleModifier.Clone())));
                
                break;
            
            case ParticleTrigger trigger:
                WeakReferenceMessenger.Default.Send(
                    new TriggerPropertyChangedMessage(trigger.Clone()));
                
                break;
            
            default:
            {
                var message = $"Unknown type of backing object: {_backingInstance.GetType()}";
                throw new InvalidOperationException(message);
            }
        }
        
        _isUpdating = false;
    }
}