using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.EmitterManagement;
using Parme.Net.Editor.Messages;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class EmitterSettingsViewModel : ObservableObject, 
    IRecipient<EmitterSettingsChanged>,
    IRecipient<TriggerChanged>
{
    private bool _isUpdating;
    
    [ObservableProperty] private float _maxParticleLifetime;
    [ObservableProperty] private string _triggerDescription = "";
    
    public ICommand SelectTriggerCommand { get; }
    
    public EmitterSettingsViewModel()
    {
        PropertyChanged += OnPropertyChanged;
        SelectTriggerCommand = new RelayCommand(() =>
        {
            WeakReferenceMessenger.Default.Send(new TriggerSelectedMessage(null));
        });
        
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
        if (_isUpdating)
        {
            return;
        }
        
        MaxParticleLifetime = settings.MaxParticleLifetime;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isUpdating)
        {
            return;
        }

        if (e.PropertyName == nameof(MaxParticleLifetime))
        {
            SettingsChanged();
        }
    }
    
    private void SettingsChanged()
    {
        if (_isUpdating)
        {
            return;
        }

        _isUpdating = true;
        var emitterSettings = new EmitterSettings(_maxParticleLifetime);
        WeakReferenceMessenger.Default.Send(new UpdatedEmitterSettings(emitterSettings));
        _isUpdating = false;
    }
}