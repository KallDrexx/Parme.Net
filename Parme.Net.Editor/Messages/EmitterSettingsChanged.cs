using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class EmitterSettingsChanged : ValueChangedMessage<EmitterSettings>
{
    public EmitterSettingsChanged(EmitterSettings value) : base(value)
    {
    }
}