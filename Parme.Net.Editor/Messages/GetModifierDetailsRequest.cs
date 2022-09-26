using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class GetModifierDetailsRequest : RequestMessage<TaggedModifier?>
{
    public Guid ItemId { get; }
    
    public GetModifierDetailsRequest(Guid itemId)
    {
        ItemId = itemId;
    }
}