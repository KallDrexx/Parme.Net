using System;
using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// Initializes particles to start rotated by a certain amount.  This rotation will be relative to the rotation
    /// of the emitter
    /// </summary>
    public class RotationInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        /// <summary>
        /// Minimum rotation (in degrees) that a particle should be initially rotated at
        /// </summary>
        public int MinDegrees { get; set; }
        
        /// <summary>
        /// Maximum rotation (in degrees) that a particle should be initially rotated at
        /// </summary>
        public int MaxDegrees { get; set; }
        
        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.RotationInRadians,
        });

        public RotationInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new RotationInitializer(_random)
            {
                MinDegrees = MinDegrees,
                MaxDegrees = MaxDegrees,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, int firstIndex,
            int lastIndex)
        {
            var rotation = particles.GetPropertyValues<float>(StandardParmeProperties.RotationInRadians.Name);

            foreach (var index in newParticleIndices)
            {
                rotation[index] = (float) (MinDegrees + _random.NextDouble() * (MaxDegrees - MinDegrees));
            }
        }
    }
}