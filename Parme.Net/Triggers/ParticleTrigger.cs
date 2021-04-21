namespace Parme.Net.Triggers
{
    /// <summary>
    /// Defines when and how particles should be created.
    /// </summary>
    public interface IParticleTrigger
    {
        /// <summary>
        /// Determines how many particles should be created
        /// </summary>
        /// <param name="particleEmitter">The particle emitter the trigger is tied to</param>
        /// <param name="timeSinceLastFrame">How much time since the last frame occurred</param>
        int DetermineNumberOfParticlesToCreate(ParticleEmitter particleEmitter, float timeSinceLastFrame);
    }
}