using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class ItemSelectedMessage : ValueChangedMessage<Guid?>
{
    public ItemSelectedMessage(Guid? value) : base(value)
    {
    }
}