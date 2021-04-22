using System;
using System.Numerics;

namespace Parme.Net.Triggers
{
    /// <summary>
    /// Emits particles after the emitter has moved a specified distance
    /// </summary>
    public class DistanceBasedTrigger : ParticleTrigger
    {
        private readonly Random _random;
        private Vector2? _previousEmitterPosition;
        private float _distanceSinceLastEmission;

        public DistanceBasedTrigger(Random random)
        {
            _random = random;
        }

        /// <summary>
        /// How far the emitter should travel before particles are emitted
        /// </summary>
        public float DistanceBetweenEmissions { get; set; }

        public override ParticleTrigger Clone()
        {
            return new DistanceBasedTrigger(_random)
            {
                DistanceBetweenEmissions = DistanceBetweenEmissions,
            };
        }

        public override int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame)
        {
            var result = 0;
            if (_previousEmitterPosition != null)
            {
                var distance = (particleEmitter.WorldCoordinates - _previousEmitterPosition.Value).Length();
                _distanceSinceLastEmission += distance;


                if (_distanceSinceLastEmission >= DistanceBetweenEmissions)
                {
                    result = _random.Next(MinParticlesToEmit, MaxParticlesToEmit + 1);
                    while (_distanceSinceLastEmission >= DistanceBetweenEmissions)
                    {
                        _distanceSinceLastEmission -= DistanceBetweenEmissions;
                    }
                }
            }

            _previousEmitterPosition = particleEmitter.WorldCoordinates;

            return result;
        }
    }
}