using System.Collections.Generic;
using Parme.Net.Behaviors;
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
        public List<ParticleBehavior> Behaviors { get; } = new();
        
        /// <summary>
        /// The starting capacity of emitters created with this configuration.  If `null` then the emitter will try
        /// to estimate its own capacity requirements.
        /// </summary>
        public int? InitialCapacity { get; set; }
    }
}