using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.Messages;

public class TriggerChanged : ValueChangedMessage<ParticleTrigger?>
{
    public TriggerChanged(ParticleTrigger? value) : base(value)
    {
    }
}