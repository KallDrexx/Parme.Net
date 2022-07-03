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
        /// Initializes properties of newly created particles.  This method is given indexes to form a contiguous set
        /// of indexes for all the new particles.
        /// </summary>
        /// <param name="emitter">
        ///     The emitter that created the particles.  Relevant if the values for the particle's properties rely on
        ///     some data from the emitter (e.g. world coordinates)
        /// </param>
        /// <param name="particles">
        ///     The collection of *all* particles managed by the emitter
        /// </param>
        /// <param name="firstIndex">The index of the first particle that is new</param>
        /// <param name="lastIndex">The index of the last particle that is new</param>
        void InitializeParticles(ParticleEmitter emitter,
            ParticleCollection particles,
            int firstIndex,
            int lastIndex);
    }
}