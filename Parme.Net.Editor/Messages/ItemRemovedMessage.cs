using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class ItemRemovedMessage : ValueChangedMessage<Guid>
{
    public ItemRemovedMessage(Guid value) : base(value)
    {
    }
}