using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;
using Parme.Net.Editor.ViewModels.ItemProperties;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class ItemEditorViewModel : ObservableObject,
    IRecipient<ItemSelectedMessage>,
    IRecipient<TriggerSelectedMessage>
{
    private const string NoItemSelected = "<No Item Selected>";

    [ObservableProperty] private string _itemName = NoItemSelected;

    public ObservableCollection<ItemProperty> ItemProperties { get; } = new();

    public ItemEditorViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }
    
    public void Receive(ItemSelectedMessage message)
    {
        ItemName = NoItemSelected;

        if (message.Value != null)
        {
            var modifier = WeakReferenceMessenger.Default.Send(new GetModifierDetailsRequest(message.Value.Value));
            if (modifier.Response != null)
            {
                UpdateFromModifier(modifier.Response.Modifier);
            }
            else
            {
                var initializer = WeakReferenceMessenger.Default.Send(new GetInitializerDetailsRequest(message.Value.Value));
                if (initializer.Response != null)
                {
                    UpdateFromInitializer(initializer.Response.Initializer);
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
        UpdateItemProperties(trigger);
    }

    private void UpdateFromModifier(IParticleModifier modifier)
    {
        ItemName = modifier.GetType().Name;
        UpdateItemProperties(modifier);
    }

    private void UpdateFromInitializer(IParticleInitializer initializer)
    {
        ItemName = initializer.GetType().Name;
        UpdateItemProperties(initializer);
    }

    private void UpdateItemProperties(object obj)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => x.Name != "PropertiesIRead")
            .Where(x => x.Name != "PropertiesIUpdate")
            .Where(x => x.Name != "PropertiesISet")
            .Select(x => new NonEditableProperty(x.Name, x.GetValue(obj)?.ToString()))
            .ToArray();
        
        ItemProperties.Clear();
        foreach (var property in properties)
        {
            ItemProperties.Add(property);
        }
    }
}