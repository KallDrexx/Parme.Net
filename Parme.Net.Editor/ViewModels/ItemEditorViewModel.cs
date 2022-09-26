using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;
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
    }

    private void UpdateFromModifier(IParticleModifier modifier)
    {
        ItemName = modifier.GetType().Name;
    }

    private void UpdateFromInitializer(IParticleInitializer initializer)
    {
        ItemName = initializer.GetType().Name;
    }
}