using System;
using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// Gives new particles a random rotational velocity, causing them to spin each frame
    /// </summary>
    public class RotationalVelocityInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        /// <summary>
        /// Minimum number of degrees created particles should rotate per second 
        /// </summary>
        public int MinDegreesPerSecond { get; set; }
        
        /// <summary>
        /// Maximum number of degrees created particles should rotate per second
        /// </summary>
        public int MaxDegreesPerSecond { get; set; }

        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.RotationalVelocity,
        });

        public RotationalVelocityInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new RotationalVelocityInitializer(_random)
            {
                MinDegreesPerSecond = MinDegreesPerSecond,
                MaxDegreesPerSecond = MaxDegreesPerSecond,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, int firstIndex,
            int lastIndex)
        {
            var speed = particles.GetPropertyValues<float>(StandardParmeProperties.RotationalVelocity.Name);

            var min = MinDegreesPerSecond * (Math.PI / 180f);
            var max = MaxDegreesPerSecond * (Math.PI / 180f);

            foreach (var index in newParticleIndices)
            {
                speed[index] = (float) (min + _random.NextDouble() * (max - min));
            }
        }
    }
}