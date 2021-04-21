using System.Collections.Generic;

namespace Parme.Net.Behaviors
{
    /// <summary>
    /// Represents a composable behavior that modifies how particles are created or modified each update.  Behaviors
    /// are meant to be composable, and a single particle emitter may have many of them active at once.
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
        /// <param name="particleEmitter">The particle emitter these created particles are a part of</param>
        /// <param name="particles">
        /// The collection of all particles (not just new ones).  Care should be taken to not change values of
        /// existing particles, unless it is intentional
        /// </param>
        /// <param name="createdParticleIndices">
        /// Indices of particles that have been created, and are ready for initialization
        /// </param>
        public abstract void InitializeCreatedParticles(ParticleEmitter particleEmitter, ParticleCollection particles, IReadOnlyList<int> createdParticleIndices);

        /// <summary>
        /// Updates property values for existing particles based on a time step.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter the particles belong to</param>
        /// <param name="particles">The collection of particles available to be modified</param>
        /// <param name="timeSinceLastFrame">How many seconds since the last frame</param>
        public abstract void UpdateParticles(ParticleEmitter particleEmitter, ParticleCollection particles, float timeSinceLastFrame);
    }
}