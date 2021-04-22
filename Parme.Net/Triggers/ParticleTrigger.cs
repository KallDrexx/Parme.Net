namespace Parme.Net.Triggers
{
    /// <summary>
    /// Defines when and how particles should be created.
    /// </summary>
    public abstract class ParticleTrigger
    {
        /// <summary>
        /// Inclusive minimum number of particles to emit
        /// </summary>
        public int MinParticlesToEmit { get; set; }
        
        /// <summary>
        /// Inclusive maximum number of particles to emit
        /// </summary>
        public int MaxParticlesToEmit { get; set; }

        /// <summary>
        /// Creates a new instance of the trigger with the same public property values as the current instance, so
        /// that it can be used with a new emitter.  It is expected that all emitter specific tracked values
        /// (e.g. time since last emission) is reset for the new instance.
        /// </summary>
        /// <returns></returns>
        public abstract ParticleTrigger Clone();
        
        /// <summary>
        /// Determines how many particles should be created
        /// </summary>
        /// <param name="particleEmitter">The particle emitter the trigger is tied to</param>
        /// <param name="timeSinceLastFrame">How much time since the last frame occurred</param>
        public abstract int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame);
    }
}