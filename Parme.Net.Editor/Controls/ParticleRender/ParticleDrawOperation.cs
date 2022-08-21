using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class ParticleDrawOperation : ICustomDrawOperation
{
    private readonly FormattedText _noSkia;
    
    public Rect Bounds { get; set; }

    public ParticleDrawOperation()
    {
        _noSkia = new FormattedText()
        {
            Text = "Current rendering API is not Skia"
        };
    }
    
    public bool HitTest(Point p) => false;
    public bool Equals(ICustomDrawOperation? other) => false;
    
    public void Dispose()
    {
    }
    
    public void Render(IDrawingContextImpl context)
    {
        var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
        if (canvas == null)
        {
            context.DrawText(Brushes.Black, new Point(), _noSkia.PlatformImpl);
        }
        else
        {
            canvas.Save();
            canvas.Clear(SKColors.Black);

            var paint = new SKPaint();
            paint.Color = SKColors.Chartreuse;
            canvas.DrawRect(0, 0, 100, 100, paint);
            
            canvas.Restore();
        }
    }
}