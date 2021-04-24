using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    /// <summary>
    /// A system that initializes one or more properties of particles when they are first emitted
    /// </summary>
    public interface IParticleInitializer
    {
        /// <summary>
        /// A collection of properties that this initializer will set on newly created particles
        /// </summary>
        HashSet<ParticleProperty> PropertiesISet { get; }

        /// <summary>
        /// Creates a new instance of the initializer that can be used with a new emitter.  It is expected that any
        /// values that the initializer tracks at runtime (i.e. values tracked to a specific emitter) are reset for
        /// the cloned instance.
        /// </summary>
        /// <returns></returns>
        IParticleInitializer Clone();

        /// <summary>
        /// Initializes properties of newly created particles
        /// </summary>
        /// <param name="emitter">
        /// The emitter that created the particles.  Relevant if the values for the particle's properties rely on
        /// some data from the emitter (e.g. world coordinates)
        /// </param>
        /// <param name="particles">
        /// The collection of *all* particles managed by the emitter
        /// </param>
        /// <param name="newParticleIndices">
        /// The indices from the particle collection for the particles that need to be initialized.  Only particle
        /// indices in this list should have their values set.
        /// </param>
        void InitializeParticles(ParticleEmitter emitter,
            ParticleCollection particles,
            IReadOnlyList<int> newParticleIndices);
    }
}