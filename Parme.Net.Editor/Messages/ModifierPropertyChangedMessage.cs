using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class ModifierPropertyChangedMessage : ValueChangedMessage<TaggedModifier>
{
    public ModifierPropertyChangedMessage(TaggedModifier value) : base(value)
    {
    }
}