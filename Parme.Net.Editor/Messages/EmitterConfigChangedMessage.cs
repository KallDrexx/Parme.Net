using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Parme.Net.Editor.Messages;

public class EmitterConfigChangedMessage : ValueChangedMessage<EmitterConfig?>
{
    public EmitterConfigChangedMessage(EmitterConfig? value) : base(value)
    {
    }
}

public class TestMessage : ValueChangedMessage<string?>
{
    public TestMessage(string? value) : base(value)
    {
    }
}