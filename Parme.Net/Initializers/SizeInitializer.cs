using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// Initializes the height and width of newly created particles
    /// </summary>
    public class SizeInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        /// <summary>
        /// Minimum height and width of newly created particles
        /// </summary>
        public Vector2 MinSize { get; set; }
        
        /// <summary>
        /// Maximum height and width of newly created particles
        /// </summary>
        public Vector2 MaxSize { get; set; }

        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.InitialHeight,
            StandardParmeProperties.InitialWidth,
            StandardParmeProperties.CurrentHeight,
            StandardParmeProperties.CurrentWidth,
        });

        public SizeInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new SizeInitializer(_random)
            {
                MinSize = MinSize,
                MaxSize = MaxSize,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, IReadOnlyList<int> newParticleIndices)
        {
            var minHeight = Math.Min(MinSize.Y, MaxSize.Y);
            var maxHeight = Math.Max(MinSize.Y, MaxSize.Y);
            var minWidth = Math.Min(MinSize.X, MaxSize.X);
            var maxWidth = Math.Max(MinSize.X, MaxSize.X);
            
            var initialHeight = particles.GetPropertyValues<float>(StandardParmeProperties.InitialHeight);
            var initialWidth = particles.GetPropertyValues<float>(StandardParmeProperties.InitialWidth);
            var currentHeight = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight);
            var currentWidth = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth);
            
            foreach (var index in newParticleIndices)
            {
                var height = minHeight + _random.NextDouble() * (maxHeight - minHeight);
                var width = minWidth + _random.NextDouble() * (maxWidth - minWidth);

                initialHeight[index] = (float) height;
                initialWidth[index] = (float) width;
                
                currentHeight[index] = (float) height;
                currentWidth[index] = (float) width;
            }
        }
    }
}