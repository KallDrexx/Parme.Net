using System;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class GetInitializerDetailsRequest : RequestMessage<TaggedInitializer?>
{
    public GetInitializerDetailsRequest(Guid itemId)
    {
        ItemId = itemId;
    }
    
    public Guid ItemId { get; }
}