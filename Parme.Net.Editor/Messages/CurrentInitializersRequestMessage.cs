using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Parme.Net.Editor.EmitterManagement;

namespace Parme.Net.Editor.Messages;

public class CurrentInitializersRequestMessage : RequestMessage<IReadOnlyList<TaggedInitializer>>
{
    
}