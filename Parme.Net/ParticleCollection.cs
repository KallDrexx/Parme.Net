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
        
        /// <summary>
        /// Defines which properties that are available to be mutated by consumers
        /// </summary>
        public ISet<ParticleProperty>? ValidPropertiesToSet { get; internal set; }
        
        /// <summary>
        /// Defines which properties consumers can get the values of, but cannot modify
        /// </summary>
        public ISet<ParticleProperty>? ValidPropertiesToRead { get; internal set; }
        
        /// <summary>
        /// The number of particles in the collection
        /// </summary>
        public int Count => _reservation.Length;
        
        internal ParticleCollection(ParticleAllocator.Reservation reservation)
        {
            _reservation = reservation;
        }

        /// <summary>
        /// Gets the values of a specific property for this set of particles.
        ///
        /// The collection of properties should *NOT* be stored for more than the current scope of operation.  The
        /// underlying collection of particle properties can (and will) be moved to new memory locations at any time,
        /// and when this happens the result of this method call will point to old values in memory.  Not only will
        /// this cause memory leaks, any updates will not affect the real particles.
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
            if (ValidPropertiesToSet?.Contains(new ParticleProperty(typeof(T), propertyName)) != true)
            {
                var message = $"This particle collection does not have '{propertyName}' as a valid property for type " +
                              $"'{typeof(T).Name}'.  Make sure the particle behavior has registered this property for this" +
                              $"operation";
                
                throw new InvalidOperationException(message);
            }

            return _reservation.GetPropertyValues<T>(propertyName);
        }

        /// <summary>
        /// Returns a read only collection of values for the specified property of the set of particles
        ///
        /// The collection of properties should *NOT* be stored for more than the current scope of operation.  The
        /// underlying collection of particle properties can (and will) be moved to new memory locations at any time,
        /// and when this happens the result of this method call will point to old values in memory.  Not only will
        /// this cause memory leaks, any updates will not affect the real particles.
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
        public ReadOnlySpan<T> GetReadOnlyPropertyValues<T>(string propertyName)
        {
            var propertyToFind = new ParticleProperty(typeof(T), propertyName);
            
            if (ValidPropertiesToRead?.Contains(propertyToFind) != true)
            {
                var message = $"This particle collection does not have '{propertyName}' as a valid property for type " +
                              $"'{typeof(T).Name}'.  Make sure the particle behavior has registered this property for this" +
                              $"operation";
                
                throw new InvalidOperationException(message);
            }

            return _reservation.GetPropertyValues<T>(propertyName);
        }

        internal void RegisterProperties()
        {
            if (ValidPropertiesToRead != null)
            {
                foreach (var property in ValidPropertiesToRead)
                {
                    _reservation.RegisterProperty(property.Type, property.Name);
                }
            }

            if (ValidPropertiesToSet != null)
            {
                foreach (var property in ValidPropertiesToSet)
                {
                    _reservation.RegisterProperty(property.Type, property.Name);
                }
            }
        }
    }
}