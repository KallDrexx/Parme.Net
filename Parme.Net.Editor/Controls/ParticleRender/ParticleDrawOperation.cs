using System.Collections.Concurrent;
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
    private readonly FormattedText _noSkiaText;
    private readonly ParticleAllocator _particleAllocator;
    private readonly SkiaParticleRenderer _particleRenderer;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly LineGridRuntime _lineGrid;
    private readonly ObservableCollection<BindableGraphicalUiElement> _gumElementsInternal = new();
    private ParticleEmitter? _emitter;
    private ParticleCollection? _particleCollection;

    public Rect Bounds { get; set; }
    public SystemManagers SystemManagers { get; }
    public ConcurrentQueue<EmitterConfig?> EmitterChanges { get; } = new();

    public ParticleDrawOperation()
    {
        _noSkiaText = new FormattedText()
        {
            Text = "Current rendering API is not Skia"
        };

        _particleAllocator = new ParticleAllocator(100);
        _particleRenderer = new SkiaParticleRenderer();
        
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

        _gumElementsInternal.Add(_lineGrid);
    }

    public bool HitTest(Point p) => false;
    public bool Equals(ICustomDrawOperation? other) => false;

    public void Render(IDrawingContextImpl context)
    {
        var elapsed = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        // If a new emitter config has been give, swap the emitter to it
        SwitchToLatestEmitterConfig();

        _emitter?.Update((float)elapsed);
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

    public void Dispose()
    {
    }

    private void RenderNonSkiaContext(IDrawingContextImpl context)
    {
        context.DrawText(Brushes.Black, new Point(), _noSkiaText.PlatformImpl);
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

        SystemManagers.Renderer.Draw(_gumElementsInternal, SystemManagers);
        
        if (_particleCollection != null)
        {
            _particleRenderer.Render(canvas, Bounds, _particleCollection);
        }

        canvas.Restore();
    }

    private void ForceGumLayout()
    {
        var wasSuspended = GraphicalUiElement.IsAllLayoutSuspended;
        GraphicalUiElement.IsAllLayoutSuspended = false;
        foreach (var item in this._gumElementsInternal)
        {
            item.UpdateLayout();
        }

        GraphicalUiElement.IsAllLayoutSuspended = wasSuspended;
    }

    private void SwitchToLatestEmitterConfig()
    {
        if (EmitterChanges.IsEmpty)
        {
            return;
        }

        _particleCollection = null;
        _emitter?.Dispose();
        
        EmitterConfig? config = null;
        while (EmitterChanges.TryDequeue(out var nextConfig))
        {
            config = nextConfig;
        }

        if (config != null)
        {
            _emitter = new ParticleEmitter(_particleAllocator, config);

            var properties = SkiaParticleRenderer.PropertiesIRead;
            _particleCollection = _emitter.CreateParticleCollection(properties, null);
        }
    }
}