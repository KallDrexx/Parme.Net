using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class UpdatedEmitterSettings : ValueChangedMessage<EmitterSettings>
{
    public UpdatedEmitterSettings(EmitterSettings value) : base(value)
    {
    }
}