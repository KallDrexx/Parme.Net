using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;

namespace Parme.Net.Editor
{
    public partial class App : Application
    {
        private TestReceiver _receiver;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            _receiver = new TestReceiver();
            WeakReferenceMessenger.Default.Register(_receiver);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class TestReceiver : IRecipient<TestMessage>
    {
        public void Receive(TestMessage message)
        {
            WeakReferenceMessenger.Default.Send(new EmitterConfigChangedMessage(null));
        }
    }
}