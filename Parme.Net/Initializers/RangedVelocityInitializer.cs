using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// Initializes the X and Y velocities of newly created particles to values within a specific range
    /// </summary>
    public class RangedVelocityInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        /// <summary>
        /// The minimum X and Y velocity values that a particle should get
        /// </summary>
        public Vector2 MinVelocity { get; set; }
        
        /// <summary>
        /// The maximum X and Y velocity values that a particle should get
        /// </summary>
        public Vector2 MaxVelocity { get; set; }

        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.VelocityX,
            StandardParmeProperties.VelocityY,
        });

        public RangedVelocityInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new RangedVelocityInitializer(_random)
            {
                MinVelocity = MinVelocity,
                MaxVelocity = MaxVelocity,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, int firstIndex,
            int lastIndex)
        {
            var velocityX = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
            var velocityY = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityY.Name);
            
            foreach (var index in newParticleIndices)
            {
                velocityX[index] = (float) (MinVelocity.X + _random.NextDouble() * (MaxVelocity.X - MinVelocity.X));
                velocityY[index] = (float) (MinVelocity.Y + _random.NextDouble() * (MaxVelocity.Y - MinVelocity.Y));
            }
        }
    }
}