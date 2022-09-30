using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class InitializerPropertyChangedMessage : ValueChangedMessage<TaggedInitializer>
{
    public InitializerPropertyChangedMessage(TaggedInitializer value) : base(value)
    {
    }
}