using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    /// <summary>
    /// System that updates the properties of particles every update call / frame.
    /// </summary>
    public interface IParticleModifier
    {
        /// <summary>
        /// Properties of particles that this modifier reads to make calculations
        /// </summary>
        HashSet<ParticleProperty> PropertiesIRead { get; }
        
        /// <summary>
        /// Properties of particles that this modifier will change each frame / update call
        /// </summary>
        HashSet<ParticleProperty> PropertiesIUpdate { get; }

        /// <summary>
        /// Creates a brand new instance of this modifier for use with a new particle emitter.  The new instance
        /// will retain all configuration related values, while internal values computed at runtime will be reset.
        /// </summary>
        public IParticleModifier Clone();

        /// <summary>
        /// Updates all particles based on how much time has elapsed
        /// </summary>
        public void Update(ParticleEmitter emitter,
            ParticleCollection particles,
            float secondsSinceLastUpdate);
    }
}