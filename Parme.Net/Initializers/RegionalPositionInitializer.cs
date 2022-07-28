using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// An initializer which positions new particles in a random area relative to the emitter
    /// </summary>
    public class RegionalPositionInitializer : IParticleInitializer
    {
        private readonly Random _random;
        
        /// <summary>
        /// The smallest X and Y coordinates the particle can be spawned at, relative to the emitter.  It is expected
        /// that the maximum X and Y values specified are both larger than the minimum X and Y values.
        /// </summary>
        public Vector2 MinRelativePosition { get; set; }
        
        /// <summary>
        /// The largest X and Y coordinates the particle can be spawned at, relative to the emitter.  It is expected
        /// that the maximum X and Y values specified are both larger than the minimum X and Y values.
        /// </summary>
        public Vector2 MaxRelativePosition { get; set; }
        
        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.PositionX,
            StandardParmeProperties.PositionY,
        });

        public RegionalPositionInitializer(Random random)
        {
            _random = random;
        }
        
        public IParticleInitializer Clone()
        {
            return new RegionalPositionInitializer(_random)
            {
                MinRelativePosition = MinRelativePosition,
                MaxRelativePosition = MaxRelativePosition,
            };
        }

        public void InitializeParticles(
            ParticleEmitter emitter, 
            ParticleCollection particles, 
            int firstIndex,
            int lastIndex)
        {
            var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
           
            for (var index = firstIndex; index <= lastIndex; index++)
            {
                var x = MinRelativePosition.X + _random.NextDouble() * (MaxRelativePosition.X - MinRelativePosition.X);
                var y = MinRelativePosition.Y + _random.NextDouble() * (MaxRelativePosition.Y - MinRelativePosition.Y);

                positionX[index] = (float) x;
                positionY[index] = (float) y;
            }
        }
    }
}