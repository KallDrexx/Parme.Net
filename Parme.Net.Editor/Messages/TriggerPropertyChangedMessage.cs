using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.Messages;

public class TriggerPropertyChangedMessage : ValueChangedMessage<ParticleTrigger>
{
    public TriggerPropertyChangedMessage(ParticleTrigger value) : base(value)
    {
    }
}