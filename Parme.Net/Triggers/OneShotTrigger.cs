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
            return new OneShotTrigger(_random)
            {
                MinParticlesToEmit = MinParticlesToEmit,
                MaxParticlesToEmit = MaxParticlesToEmit,
            };
        }

        public override int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame)
        {
            particleEmitter.IsEmittingNewParticles = false;

            var min = Math.Min(MinParticlesToEmit, MaxParticlesToEmit);
            var max = Math.Max(MinParticlesToEmit, MaxParticlesToEmit);

            return _random.Next(min, max + 1);
        }
    }
}