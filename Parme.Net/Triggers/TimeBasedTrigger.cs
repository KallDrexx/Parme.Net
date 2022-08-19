using System;

namespace Parme.Net.Triggers
{
    public class TimeBasedTrigger : ParticleTrigger
    {
        private readonly Random _random;
        private float _timeSinceLastEmission;
        private float _secondsBetweenEmissions;

        public TimeBasedTrigger(Random random)
        {
            _random = random;
        }

        /// <summary>
        /// The number of seconds that should occur before another set of particles is emitted
        /// </summary>
        public float SecondsBetweenEmissions
        {
            get => _secondsBetweenEmissions;
            set
            {
                _secondsBetweenEmissions = value;
                _timeSinceLastEmission = value; // Cause it to trigger immediately
            }
        }

        public override ParticleTrigger Clone()
        {
            return new TimeBasedTrigger(_random)
            {
                SecondsBetweenEmissions = SecondsBetweenEmissions,
                MinParticlesToEmit = MinParticlesToEmit,
                MaxParticlesToEmit = MaxParticlesToEmit,
            };
        }

        public override int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame)
        {
            var result = 0;
            
            _timeSinceLastEmission += timeSinceLastFrame;
            if (_timeSinceLastEmission >= SecondsBetweenEmissions)
            {
                result = _random.Next(MinParticlesToEmit, MaxParticlesToEmit + 1);

                while (_timeSinceLastEmission >= SecondsBetweenEmissions)
                {
                    _timeSinceLastEmission -= SecondsBetweenEmissions;
                }
            }

            return result;
        }
    }
}