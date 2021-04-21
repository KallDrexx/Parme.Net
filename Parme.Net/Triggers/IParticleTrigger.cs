namespace Parme.Net.Triggers
{
    /// <summary>
    /// Defines when and how particles should be created.
    /// </summary>
    public interface IParticleTrigger
    {
        /// <summary>
        /// Inclusive minimum number of particles to emit
        /// </summary>
        int MinParticlesToEmit { get; set; }
        
        /// <summary>
        /// Inclusive maximum number of particles to emit
        /// </summary>
        int MaxParticlesToEmit { get; set; }
        
        /// <summary>
        /// Determines how many particles should be created
        /// </summary>
        /// <param name="particleEmitter">The particle emitter the trigger is tied to</param>
        /// <param name="timeSinceLastFrame">How much time since the last frame occurred</param>
        int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame);
    }
}