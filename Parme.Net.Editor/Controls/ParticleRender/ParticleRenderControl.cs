using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class ParticleRenderControl : Control
{
    private readonly ParticleDrawOperation _drawOperation;
    
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