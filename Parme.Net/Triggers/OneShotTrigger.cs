using System;

namespace Parme.Net.Triggers
{
    /// <summary>
    /// Triggers particles once, and then tells the emitter to stop emitting
    /// </summary>
    public class OneShotTrigger : IParticleTrigger
    {
        private readonly Random _random;

        public OneShotTrigger(Random random)
        {
            _random = random;
        }
        
        public int MinParticlesToEmit { get; set; }
        
        public int MaxParticlesToEmit { get; set; }
        
        public int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame)
        {
            particleEmitter.IsEmittingNewParticles = false;

            return _random.Next(MinParticlesToEmit, MaxParticlesToEmit + 1);
        }
    }
}