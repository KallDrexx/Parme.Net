using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor.ViewModels;

public partial class EmitterRenderViewModel : ObservableObject, IRecipient<EmitterConfigChangedMessage>
{
    [ObservableProperty] private EmitterConfig? _currentEmitterConfig;

    public EmitterRenderViewModel()
    {
        CurrentEmitterConfig = WeakReferenceMessenger.Default
            .Send<CurrentEmitterConfigRequestMessage>();
        
        WeakReferenceMessenger.Default.Register(this);
    }

    public void Receive(EmitterConfigChangedMessage message)
    {
        CurrentEmitterConfig = message.Value;
    }
}