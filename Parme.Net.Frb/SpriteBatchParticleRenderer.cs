using System.Collections.Generic;
using FlatRedBall;
using Microsoft.Xna.Framework;

namespace Parme.Net.Frb;

using Microsoft.Xna.Framework.Graphics;

public class SpriteBatchParticleRenderer
{
    private readonly Texture2D _texture;
    private readonly ParticleCollection _particleCollection;
    
    public SpriteBatchParticleRenderer(
        string textureFilePath,
        ParticleCollection particleCollection)
    {
        _particleCollection = particleCollection;

        _texture = string.IsNullOrWhiteSpace(textureFilePath)
            ? GetDefaultWhiteTexture()
            : FlatRedBallServices.Load<Texture2D>(textureFilePath);
    }
    
    public void Render(
        ParticleCamera camera, 
        SpriteBatch spriteBatch,
        ref TextureSectionCoords[] textureSections)
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
        
        var textureSectionIndex =
            _particleCollection.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.TextureSectionIndex.Name);
        
        for (var index = 0; index < _particleCollection.Count; index++)
        {
            if (!isAlive[index])
            {
                continue;
            }
            
            var startX = positionX[index];
            var startY = -positionY[index]; // positive y points up

            var destinationRectangle = new Rectangle((int)startX,
                (int)startY,
                (int)width[index],
                (int)height[index]);

            Rectangle sourceRectangle;
            if (textureSections.Length == 0)
            {
                sourceRectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);
            }
            else
            {
                ref var section = ref textureSections[textureSectionIndex[index]];
                sourceRectangle = new Rectangle(section.LeftX,
                    section.TopY,
                    section.RightX - section.LeftX,
                    section.BottomY - section.TopY);
            }

            var colorModifier = new Color((byte)red[index],
                (byte)green[index],
                (byte)blue[index],
                (byte)alpha[index]);
            
            spriteBatch.Draw(_texture,
                destinationRectangle,
                sourceRectangle,
                colorModifier,
                -rotation[index], // CCW rotations
                new Vector2(sourceRectangle.Width / 2, sourceRectangle.Height / 2),
                SpriteEffects.None,
                0f);
        }
    }
    
    private static Texture2D GetDefaultWhiteTexture()
    {
        const int size = 10;
        var pixels = new Color[size * size];
        for (var x = 0; x < pixels.Length; x++)
        {
            pixels[x] = Color.White;
        }
            
        var texture = new Texture2D(FlatRedBallServices.GraphicsDevice, size, size);
        texture.SetData(pixels);

        return texture;
    }
}