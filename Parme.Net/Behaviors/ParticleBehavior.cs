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
        /// Gets the nearest multiple of the given number.  Useful for SIMD scenarios
        /// </summary>
        protected static int NearestMultiple(int number, int multiple) => ((number - 1) | (multiple - 1)) + 1 - multiple;
        
        /// <summary>
        /// Properties that this behavior will set an initial value for when a new particle is created.. 
        /// </summary>
        public virtual HashSet<ParticleProperty>? InitializedProperties => null;

        /// <summary>
        /// Properties that this behavior will update each time the emitter's Update() method is called.
        /// </summary>
        public virtual HashSet<ParticleProperty>? ModifiedProperties => null;

        /// <summary>
        /// Creates a copy of the behavior so that it may be attached to a new emitter.  All data for the behavior
        /// is copied *except* data tracked against a specific emitter, all emitter specific data will be reset. 
        /// </summary>
        /// <returns></returns>
        public abstract ParticleBehavior Clone();

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
        public virtual void InitializeCreatedParticles(ParticleEmitter particleEmitter, 
            ParticleCollection particles,
            IReadOnlyList<int> createdParticleIndices)
        {
            
        }

        /// <summary>
        /// Updates property values for existing particles based on a time step.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter the particles belong to</param>
        /// <param name="particles">The collection of particles available to be modified</param>
        /// <param name="timeSinceLastFrame">How many seconds since the last frame</param>
        public virtual void UpdateParticles(ParticleEmitter particleEmitter,
            ParticleCollection particles,
            float timeSinceLastFrame)
        {
            
        }
    }
}