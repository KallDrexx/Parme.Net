using System.Collections.Generic;
using Avalonia;
using SkiaSharp;

namespace Parme.Net.Editor.Controls.ParticleRender;

public class SkiaParticleRenderer
{
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

    public void Render(SKCanvas canvas, Rect bounds, ParticleCollection particleCollection)
    {
        var isAlive = particleCollection.GetReadOnlyPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
        var positionX = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.PositionX.Name);
        var positionY = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.PositionY.Name);
        var width = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
        var height = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
        var red = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentRed.Name);
        var green = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentGreen.Name);
        var blue = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentBlue.Name);
        var alpha = particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.CurrentAlpha.Name);
        var rotation =
            particleCollection.GetReadOnlyPropertyValues<float>(StandardParmeProperties.RotationInRadians.Name);

        var center = bounds.Center;
        var paint = new SKPaint();

        for (var index = 0; index < particleCollection.Count; index++)
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