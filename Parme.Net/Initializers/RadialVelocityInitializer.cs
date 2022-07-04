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
            var radianRandoms = new double[count];
            var magnitudeRandoms = new double[count];

            _random.NextDoubles(radianRandoms);
            _random.NextDoubles(magnitudeRandoms);

            int x;
            var offset = Vector<float>.Count;
            var nearestMultiple = SimdUtils.NearestMultiple(count, offset);

            for (x = 0; x < nearestMultiple; x += offset)
            {
                var xSlice = velocityX.Slice(x, offset);
                var ySlice = velocityY.Slice(x, offset);
                
                var radians = new Vector<float>()
            }

            foreach (var index in newParticleIndices)
            {
                var radians = minRadians + _random.NextDouble() * (maxRadians - minRadians);
                var magnitude = MinMagnitude + _random.NextDouble() * (MaxMagnitude - MinMagnitude);
                
                // Convert from polar coordinates to cartesian coordinates
                velocityX[index] = (float) (magnitude * Math.Cos(radians));
                velocityY[index] = (float) (magnitude * Math.Sin(radians));
            }
        }
    }
}