using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class ModifiersChangedMessage : ValueChangedMessage<IReadOnlyList<TaggedModifier>>
{
    public ModifiersChangedMessage(IReadOnlyList<TaggedModifier> value) : base(value)
    {
    }
}