using System;
using System.Collections.Generic;

namespace Parme.Net
{
    /// <summary>
    /// Represents a set of particles that can be referenced or modified.
    /// </summary>
    public class ParticleCollection
    {
        private readonly ParticleAllocator.Reservation _reservation;
        
        public HashSet<ParticleProperty>? ValidProperties { get; internal set; }
        
        /// <summary>
        /// The number of particles in the collection
        /// </summary>
        public int Count => _reservation.Length;
        
        internal ParticleCollection(ParticleAllocator.Reservation reservation)
        {
            _reservation = reservation;
        }

        /// <summary>
        /// Gets the values of a specific property for this set of particles
        /// </summary>
        /// <param name="propertyName">Name of the property to get values for</param>
        /// <typeparam name="T">The type for the property.  This must match the type the property was registered with</typeparam>
        /// <returns>Collection of values for each particle in the collection</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property is not listed as a valid property for this collection.  This is done to ensure that
        /// behaviors are correctly registering the properties they rely on.  Without this check this operation becomes
        /// non-deterministic, as it might sometimes succeed and sometimes fail depending on what other emitters or
        /// behaviors have been created prior to this operation.
        /// </exception>
        public Span<T> GetPropertyValues<T>(string propertyName)
        {
            if (ValidProperties?.Contains(new ParticleProperty(typeof(T), propertyName)) != true)
            {
                var message = $"This particle collection does not have '{propertyName}' as a valid property for type " +
                              $"'{typeof(T).Name}'.  Make sure the particle behavior has registered this property for this" +
                              $"operation";
                
                throw new InvalidOperationException(message);
            }

            return _reservation.GetPropertyValues<T>(propertyName);
        }
    }
}