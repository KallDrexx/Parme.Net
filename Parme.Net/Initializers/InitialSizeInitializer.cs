using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Initializers
{
    public class InitialSizeInitializer : IParticleInitializer
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

        public InitialSizeInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new InitialSizeInitializer(_random)
            {
                MinSize = MinSize,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, IReadOnlyList<int> newParticleIndices)
        {
            float minHeight, maxHeight, minWidth, maxWidth;
            if (MinSize.Y < MaxSize.Y)
            {
                minHeight = MinSize.Y;
                maxHeight = MaxSize.Y;
            }
            else
            {
                minHeight = MaxSize.Y;
                maxHeight = MinSize.Y;
            }

            if (MinSize.X < MaxSize.X)
            {
                minWidth = MinSize.X;
                maxWidth = MaxSize.X;
            }
            else
            {
                minWidth = MaxSize.X;
                maxWidth = MinSize.X;
            }

            var initialHeight = particles.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = particles.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            var currentHeight = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            
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