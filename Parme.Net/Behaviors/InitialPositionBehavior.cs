using System;
using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net.Behaviors
{
    /// <summary>
    /// Sets the initial position of the particle inside a minimum and maximum bounds around the emitter
    /// </summary>
    public class InitialPositionBehavior : ParticleBehavior
    {
        private static readonly HashSet<ParticleProperty> PropertiesToInitialize = new(new[]
        {
            StandardParmeProperties.PositionX,
            StandardParmeProperties.PositionY,
        });

        private readonly Random _random;

        public override HashSet<ParticleProperty> InitializedProperties => PropertiesToInitialize;

        /// <summary>
        /// The lower bound of positions the particle can start at relative to the emitter's position
        /// </summary>
        public Vector2 MinPositionRelativeToEmitter { get; set; }
        
        /// <summary>
        /// The upper bound of positions the particle can start at relative to the emitter's position 
        /// </summary>
        public Vector2 MaxPositionRelativeToEmitter { get; set; }

        public InitialPositionBehavior(Random random)
        {
            _random = random;
        }

        public override void InitializeCreatedParticles(ParticleEmitter particleEmitter, 
            ParticleCollection particles,
            IReadOnlyList<int> createdParticleIndices)
        {
            var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
            
            foreach (var index in createdParticleIndices)
            {
                var x = MinPositionRelativeToEmitter.X + _random.NextDouble() *
                    (MaxPositionRelativeToEmitter.X - MinPositionRelativeToEmitter.X);

                var y = MinPositionRelativeToEmitter.Y + _random.NextDouble() *
                    (MaxPositionRelativeToEmitter.Y - MinPositionRelativeToEmitter.Y);

                positionX[index] = particleEmitter.WorldCoordinates.X + (float) x;
                positionY[index] = particleEmitter.WorldCoordinates.Y + (float) y;
            }
        }
    }
}