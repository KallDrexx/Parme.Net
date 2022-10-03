using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class AddInitializerMessage : ValueChangedMessage<object?>
{
    public AddInitializerMessage() : base(null)
    {
    }
}