using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class AddModifierMessage : ValueChangedMessage<object?>
{
    public AddModifierMessage() : base(null)
    {
    }
}