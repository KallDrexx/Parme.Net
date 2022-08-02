using System;
using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    public class TextureInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        public byte FirstTextureSectionIndex { get; set; }
        public byte LastTextureSectionIndex { get; set; }

        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.TextureSectionIndex,
        });

        public TextureInitializer(Random random)
        {
            _random = random;
        }

        public IParticleInitializer Clone()
        {
            return new TextureInitializer(_random)
            {
                FirstTextureSectionIndex = FirstTextureSectionIndex,
                LastTextureSectionIndex = LastTextureSectionIndex,
            };
        }

        public void InitializeParticles(
            ParticleEmitter emitter, 
            ParticleCollection particles, 
            IReadOnlyList<int> newParticleIndices)
        {
            var textureSectionIndices =
                particles.GetPropertyValues<byte>(StandardParmeProperties.TextureSectionIndex.Name);
            
            foreach (var index in newParticleIndices)
            {
                var textureIndex = _random.Next(FirstTextureSectionIndex, LastTextureSectionIndex + 1);
                textureSectionIndices[index] = (byte) textureIndex;
            }
        }
    }
}