using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class TriggerSelectedMessage: ValueChangedMessage<object?>
{
    public TriggerSelectedMessage(object? value) : base(value)
    {
    }
}