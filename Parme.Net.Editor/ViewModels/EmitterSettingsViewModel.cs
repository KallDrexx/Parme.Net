using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class EmitterSettingsViewModel : ObservableObject, 
    IRecipient<EmitterSettingsChanged>,
    IRecipient<TriggerChanged>

{
    [ObservableProperty] private int _initialCapacity;
    [ObservableProperty] private float _maxParticleLifetime;
    [ObservableProperty] private string _triggerDescription = "";

    public EmitterSettingsViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);

        var trigger = WeakReferenceMessenger.Default.Send<CurrentTriggerRequest>();
        var settings = WeakReferenceMessenger.Default.Send<CurrentEmitterSettingsRequest>();
        
        UpdateTrigger(trigger.Response);
        UpdateSettings(settings.Response);
    }

    public void Receive(EmitterSettingsChanged message)
    {
        UpdateSettings(message.Value);
    }

    public void Receive(TriggerChanged message)
    {
        UpdateTrigger(message.Value);
    }

    private void UpdateTrigger(ParticleTrigger? trigger)
    {
        _triggerDescription = trigger != null
            ? trigger.GetType().Name
            : "<None>";
    }

    private void UpdateSettings(EmitterSettings settings)
    {
        _initialCapacity = settings.InitialCapacity;
        _maxParticleLifetime = settings.MaxParticleLifetime;
    }
}