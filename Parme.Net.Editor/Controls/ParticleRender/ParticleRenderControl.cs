using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class ParticleRenderControl : Control
{
    private readonly ParticleDrawOperation _drawOperation;
    private EmitterConfig? _testEmitterConfig;

    public static readonly DirectProperty<ParticleRenderControl, EmitterConfig?>? TestEmitterConfigProperty =
        AvaloniaProperty.RegisterDirect<ParticleRenderControl, EmitterConfig?>(
            nameof(TestEmitterConfig),
            x => x.TestEmitterConfig,
            (x, v) => x.TestEmitterConfig = v);
    
    public EmitterConfig? TestEmitterConfig
    {
        get => _testEmitterConfig;
        set
        {
            _drawOperation.EmitterChanges.Enqueue(value);
            _testEmitterConfig = value;
        }
    }

    public ParticleRenderControl()
    {
        ClipToBounds = true;

        _drawOperation = new ParticleDrawOperation();
    }

    public override void Render(DrawingContext context)
    {
        _drawOperation.Bounds = new Rect(0, 0, Bounds.Width, Bounds.Height);

        context.Custom(_drawOperation);
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

}