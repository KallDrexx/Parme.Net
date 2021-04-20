using System.Collections.Generic;

namespace Parme.Net.Behaviors
{
    /// <summary>
    /// Represents a composable behavior that modifies how particles are created or modified each update.  Behaviors
    /// are meant to be composable, and a single emitter may have many of them active at once.
    /// </summary>
    public abstract class ParticleBehavior
    {
        /// <summary>
        /// Properties that this behavior will set an initial value for when a new particle is created.  
        /// </summary>
        public abstract HashSet<ParticleProperty> InitializedProperties { get; }
        public abstract HashSet<ParticleProperty> ModifiedProperties { get; }

        /// <summary>
        /// Initializes property values for newly created particles. 
        /// </summary>
        /// <param name="emitter">The emitter these created particles are a part of</param>
        /// <param name="particles">
        /// The collection of all particles (not just new ones).  Care should be taken to not change values of
        /// existing particles, unless it is intentional
        /// </param>
        /// <param name="createdParticleIndices">
        /// Indices of particles that have been created, and are ready for initialization
        /// </param>
        public abstract void InitializeCreatedParticles(Emitter emitter, ParticleCollection particles, IReadOnlyList<int> createdParticleIndices);

        /// <summary>
        /// Updates property values for existing particles based on a time step.
        /// </summary>
        /// <param name="emitter">The emitter the particles belong to</param>
        /// <param name="particles">The collection of particles available to be modified</param>
        /// <param name="timeSinceLastFrame">How many seconds since the last frame</param>
        public abstract void UpdateParticles(Emitter emitter, ParticleCollection particles, float timeSinceLastFrame);
    }
}