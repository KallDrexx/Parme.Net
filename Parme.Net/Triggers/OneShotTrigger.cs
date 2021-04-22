using System;

namespace Parme.Net.Triggers
{
    /// <summary>
    /// Triggers particles once, and then tells the emitter to stop emitting
    /// </summary>
    public class OneShotTrigger : ParticleTrigger
    {
        private readonly Random _random;

        public OneShotTrigger(Random random)
        {
            _random = random;
        }

        public override ParticleTrigger Clone()
        {
            return new OneShotTrigger(_random);
        }

        public override int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame)
        {
            particleEmitter.IsEmittingNewParticles = false;

            return _random.Next(MinParticlesToEmit, MaxParticlesToEmit + 1);
        }
    }
}