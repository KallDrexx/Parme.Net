namespace Parme.Net.Triggers
{
    /// <summary>
    /// Defines when and how particles should be created.
    /// </summary>
    public abstract class ParticleTrigger
    {
        /// <summary>
        /// Determines how many particles should be created
        /// </summary>
        /// <param name="emitter">The emitter the trigger is tied to</param>
        /// <param name="timeSinceLastFrame">How much time since the last frame occurred</param>
        internal abstract int DetermineNumberOfParticlesToCreate(Emitter emitter, float timeSinceLastFrame);
    }
}