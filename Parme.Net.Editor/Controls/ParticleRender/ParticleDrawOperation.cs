using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Wireframe;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using SkiaGum.GueDeriving;
using SkiaSharp;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class ParticleDrawOperation : ICustomDrawOperation
{
    private readonly FormattedText _noSkia;
    private readonly ParticleEmitter _emitter;
    private readonly SkiaParticleRenderer _particleRenderer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly LineGridRuntime _lineGrid;

    public Rect Bounds { get; set; }
    public SystemManagers SystemManagers { get; private set; }
    private ObservableCollection<BindableGraphicalUiElement> GumElementsInternal { get; set; } = new();

    public ParticleDrawOperation(EmitterConfig config)
    {
        _noSkia = new FormattedText()
        {
            Text = "Current rendering API is not Skia"
        };

        var allocator = new ParticleAllocator(100);
        _emitter = new ParticleEmitter(allocator, config);

        var properties = SkiaParticleRenderer.PropertiesIRead;
        var particleCollection = _emitter.CreateParticleCollection(properties, null);

        _particleRenderer = new SkiaParticleRenderer(particleCollection);
        SystemManagers = new SystemManagers();
        SystemManagers.Initialize();
        SystemManagers.Renderer.ClearsCanvas = false;

        _lineGrid = new LineGridRuntime()
        {
            XUnits = GeneralUnitType.PixelsFromMiddle,
            YUnits = GeneralUnitType.PixelsFromMiddle,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center,
            HeightUnits = DimensionUnitType.Absolute,
            WidthUnits = DimensionUnitType.Absolute,
            CellWidth = 50,
            CellHeight = 50,
        };

        GumElementsInternal.Add(_lineGrid);
    }
    
    public void ForceGumLayout()
    {
        var wasSuspended = GraphicalUiElement.IsAllLayoutSuspended;
        GraphicalUiElement.IsAllLayoutSuspended = false;
        foreach (var item in this.GumElementsInternal)
        {
            item.UpdateLayout();
        }
        GraphicalUiElement.IsAllLayoutSuspended = wasSuspended;
    }

    public bool HitTest(Point p) => false;
    public bool Equals(ICustomDrawOperation? other) => false;

    public void Dispose()
    {
    }

    public void Render(IDrawingContextImpl context)
    {
        var elapsed = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();
        
        _emitter.Update((float) elapsed);
        
        var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
        if (canvas != null)
        {
            RenderSkiaContext(canvas);
        }
        else
        {
            RenderNonSkiaContext(context);
        }
    }

    private void RenderNonSkiaContext(IDrawingContextImpl context)
    {
        context.DrawText(Brushes.Black, new Point(), _noSkia.PlatformImpl);
    }

    private void RenderSkiaContext(SKCanvas canvas)
    {
        canvas.Save();
        canvas.Clear(SKColors.Black);
        
        SystemManagers.Canvas = canvas;

        GraphicalUiElement.CanvasWidth = (float)Bounds.Width / SystemManagers.Renderer.Camera.Zoom;
        GraphicalUiElement.CanvasHeight = (float)Bounds.Height / SystemManagers.Renderer.Camera.Zoom;

        var desiredCellWidth = 1 + (int)GraphicalUiElement.CanvasWidth / _lineGrid.CellWidth;
        if (desiredCellWidth % 2 != 0)
        {
            desiredCellWidth++;
        }

        var desiredCellHeight = 1 + (int)GraphicalUiElement.CanvasHeight / _lineGrid.CellHeight;
        if (desiredCellHeight % 2 != 0)
        {
            desiredCellHeight++;
        }

        _lineGrid.Width = desiredCellWidth * _lineGrid.CellWidth;
        _lineGrid.Height = desiredCellHeight * _lineGrid.CellHeight;

        ForceGumLayout();

        SystemManagers.Renderer.Draw(this.GumElementsInternal, SystemManagers);

        _particleRenderer.Render(canvas, Bounds);

        canvas.Restore();
    }
}