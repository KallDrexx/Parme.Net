using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// Sets an initial velocity based on the specified angle range and magnitude range
    /// </summary>
    public class RadialVelocityInitializer : IParticleInitializer
    {
        private readonly Random _random;

        /// <summary>
        /// Minimum magnitude for each created particle's velocity
        /// </summary>
        public float MinMagnitude { get; set; }
        
        /// <summary>
        /// Maximum magnitude for each created particle's velocity
        /// </summary>
        public float MaxMagnitude { get; set; }
        
        /// <summary>
        /// Smallest angle in degrees in which a particle will be ejected at
        /// </summary>
        public int MinDegrees { get; set; }
        
        /// <summary>
        /// Largest angle in degrees in which a particle will be ejected at
        /// </summary>
        public int MaxDegrees { get; set; }

        public float XAxisScale { get; set; } = 1f;
        public float YAxisScale { get; set; } = 1f;

        public RadialVelocityInitializer(Random random)
        {
            _random = random;
        }
        
        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.VelocityX,
            StandardParmeProperties.VelocityY,
        });
        
        public IParticleInitializer Clone()
        {
            return new RadialVelocityInitializer(_random)
            {
                MinDegrees = MinDegrees,
                MaxDegrees = MaxDegrees,
                MaxMagnitude = MaxMagnitude,
                MinMagnitude = MinMagnitude,
            };
        }

        public void InitializeParticles(
            ParticleEmitter emitter, 
            ParticleCollection particles, 
            int firstIndex,
            int lastIndex)
        {
            var velocityX = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
            var velocityY = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityY.Name);

            var minRadians = MinDegrees * (Math.PI / 180f);
            var maxRadians = MaxDegrees * (Math.PI / 180f);

            var count = lastIndex - firstIndex + 1;
            for (var index = firstIndex; index <= lastIndex; index++)
            {
                var randomIndex = index - firstIndex;
                var radians = maxRadians - _random.NextDouble() * (maxRadians - minRadians);
                var magnitude = MaxMagnitude - _random.NextDouble() * (MaxMagnitude - MinMagnitude);
                
                // Convert from polar coordinates to cartesian coordinates
                var x = magnitude * Math.Cos(radians) * XAxisScale;
                var y = magnitude * Math.Sin(radians) * YAxisScale;

                velocityX[index] = (float) x;
                velocityY[index] = (float) y;
            }
        }
    }
}