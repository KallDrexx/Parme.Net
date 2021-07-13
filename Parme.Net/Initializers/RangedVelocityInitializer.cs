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
        /// The minimum X and Y velocity values that 
        /// </summary>
        public Vector2 MinVelocity { get; set; }
        
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
            throw new System.NotImplementedException();
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, IReadOnlyList<int> newParticleIndices)
        {
            throw new System.NotImplementedException();
        }
    }
}