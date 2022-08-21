using System;
using System.Collections.Generic;
using Avalonia;
using SkiaSharp;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class SkiaParticleRenderer
{
    private readonly ParticleCollection _particleCollection;
    
    public static HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
    {
        StandardParmeProperties.IsAlive,
        StandardParmeProperties.PositionX,
        StandardParmeProperties.PositionY,
        StandardParmeProperties.CurrentWidth,
        StandardParmeProperties.CurrentHeight,
        StandardParmeProperties.CurrentRed,
        StandardParmeProperties.CurrentGreen,
        StandardParmeProperties.CurrentBlue,
        StandardParmeProperties.CurrentAlpha,
        StandardParmeProperties.RotationInRadians,
        StandardParmeProperties.TextureSectionIndex,
    });

    public SkiaParticleRenderer(ParticleCollection particleCollection)
    {
        _particleCollection = particleCollection;
    }

    public void Render(SKCanvas canvas, Rect bounds)
    {
        var isAlive = _particleCollection.GetReadOnlyPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
        var positionX = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.PositionX.Name);
        var positionY = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.PositionY.Name);
        var width = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
        var height = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
        var red = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentRed.Name);
        var green = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentGreen.Name);
        var blue = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentBlue.Name);
        var alpha = _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentAlpha.Name);
        var rotation =
            _particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.RotationInRadians.Name);

        var center = bounds.Center;
        var paint = new SKPaint();

        for (var index = 0; index < _particleCollection.Count; index++)
        {
            if (!isAlive[index])
            {
                continue;
            }

            var startX = positionX[index];
            var startY = -positionY[index]; // positive Y points down
            var left = -width[index] / 2;
            var top = -height[index] / 2;
            
            paint.Color = new SKColor((byte)red[index],
                (byte)green[index],
                (byte)blue[index],
                (byte)alpha[index]);

            canvas.Save();
            canvas.Translate((float)center.X + startX, (float)center.Y + startY);
            canvas.RotateRadians(-rotation[index]);
            canvas.DrawRect((int)left, (int)top, width[index], height[index], paint);
            canvas.Restore();
        }
    }
}