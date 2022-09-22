using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class InitializersChangedMessage : ValueChangedMessage<IReadOnlyList<TaggedInitializer>>
{
    public InitializersChangedMessage(IReadOnlyList<TaggedInitializer> value) : base(value)
    {
    }
}