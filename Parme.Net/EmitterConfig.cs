using System.Collections.Generic;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net
{
    /// <summary>
    /// Represents a holistic configuration for an emitter.  This makes it easy to create multiple identical
    /// emitters without accidentally sharing behavior/trigger instances
    /// </summary>
    public class EmitterConfig
    {
        public ParticleTrigger? Trigger { get; set; }
        public List<IParticleInitializer> Initializers { get; } = new();
        public List<IParticleModifier> Modifiers { get; } = new();

        /// <summary>
        /// How many seconds a single particle should stay alive for
        /// </summary>
        public float MaxParticleLifetime { get; set; }
        
        /// <summary>
        /// The starting capacity of emitters created with this configuration.  If `null` then the emitter will try
        /// to estimate its own capacity requirements.
        /// </summary>
        public int? InitialCapacity { get; set; }
    }
}