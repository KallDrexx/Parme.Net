using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Behaviors
{
    /// <summary>
    /// Sets the initial size of a particle, and interpolates the particle's size to reach the final desired size
    /// by the end of the particle's lifetime
    /// </summary>
    public class SizingBehavior : ParticleBehavior
    {
        private readonly Random _random;

        public override HashSet<ParticleProperty>? InitializedProperties { get; } = new(new[]
        {
            StandardParmeProperties.CurrentHeight,
            StandardParmeProperties.CurrentWidth,
            StandardParmeProperties.InitialHeight,
            StandardParmeProperties.InitialWidth,
        });

        public override HashSet<ParticleProperty>? ModifiedProperties { get; } = new(new[]
        {
            StandardParmeProperties.CurrentHeight,
            StandardParmeProperties.CurrentWidth,
            StandardParmeProperties.InitialHeight,
            StandardParmeProperties.InitialWidth,
            StandardParmeProperties.TimeAlive,
        });

        /// <summary>
        /// The minimum height and width of a particle when its created
        /// </summary>
        public Vector2 MinParticleInitialSize { get; set; }
        
        /// <summary>
        /// The maximum height and width of a particle when its created
        /// </summary>
        public Vector2 MaxParticleInitialSize { get; set; }
        
        /// <summary>
        /// The desired height and width of a particle when it reaches the end of it's life
        /// </summary>
        public Vector2 EndingSize { get; set; }

        public SizingBehavior(Random random)
        {
            _random = random;
        }
        
        public override ParticleBehavior Clone()
        {
            return new SizingBehavior(_random)
            {
                MinParticleInitialSize = MinParticleInitialSize,
                MaxParticleInitialSize = MaxParticleInitialSize,
                EndingSize = EndingSize,
            };
        }

        public override void InitializeCreatedParticles(ParticleEmitter particleEmitter, 
            ParticleCollection particles,
            IReadOnlyList<int> createdParticleIndices)
        {
            var currentHeight = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            var initialHeight = particles.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = particles.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);

            foreach (var index in createdParticleIndices)
            {
                var height = MinParticleInitialSize.Y + _random.NextDouble() * (MaxParticleInitialSize.Y - MinParticleInitialSize.Y);
                var width = MinParticleInitialSize.X + _random.NextDouble() * (MaxParticleInitialSize.X - MinParticleInitialSize.X);

                currentHeight[index] = (float) height;
                initialHeight[index] = (float) height;
                currentWidth[index] = (float) width;
                initialWidth[index] = (float) width;
            }
        }

        public override void UpdateParticles(ParticleEmitter particleEmitter, 
            ParticleCollection particles, 
            float timeSinceLastFrame)
        {
            var currentHeight = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            var initialHeight = particles.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = particles.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            var timeAlive = particles.GetPropertyValues<float>(StandardParmeProperties.TimeAlive.Name);

            var maxLifetimeVector = new Vector<float>(particleEmitter.MaxParticleLifetime);
            var endingWidthVector = new Vector<float>(EndingSize.X);
            var endingHeightVector = new Vector<float>(EndingSize.Y);
            
            var offset = Vector<float>.Count;
            var nearestMultiple = NearestMultiple(particles.Count, offset);
            int index;
            for (index = 0; index < nearestMultiple; index += offset)
            {
                var initialHeightVector = new Vector<float>(initialHeight.Slice(index, offset));
                var initialWidthVector = new Vector<float>(initialWidth.Slice(index, offset));
                var timeAliveVector = new Vector<float>(timeAlive.Slice(index, offset));

                var lifetimePercent = timeAliveVector / maxLifetimeVector;

                var finalWidth = initialWidthVector + lifetimePercent * (endingWidthVector - initialWidthVector);
                var finalHeight = initialHeightVector + lifetimePercent * (endingHeightVector - initialHeightVector);
                
                finalWidth.CopyTo(currentWidth.Slice(index, offset));
                finalHeight.CopyTo(currentHeight.Slice(index, offset));
            }
            
            // Calculate individually for the remaining items
            for (; index < particles.Count; index++)
            {
                var lifetimePercent = timeAlive[index] / particleEmitter.MaxParticleLifetime;
                currentWidth[index] = initialWidth[index] + lifetimePercent * (EndingSize.X - initialWidth[index]);
                currentHeight[index] = initialHeight[index] + lifetimePercent * (EndingSize.Y - initialHeight[index]);
            }
        }
    }
}