﻿using System;
using System.Collections.Generic;

namespace Parme.Net.Core
{
    /// <summary>
    /// Manages the allocation of particles and their associated properties.  This is designed to provide minimal
    /// GC pressure as emitters are created and destroyed. 
    ///
    /// When an emitter is created it will reserve an expected quantity of particles.  The emitter can expand the
    /// number of particles it has manages but does not have the ability to shrink the number of particles it manages.
    ///
    /// When an emitter is no longer needed it can be disposed, which will cause the particles to be assigned to the
    /// next emitter that gets created, even if that new emitter has different capacity requirements.  
    ///
    /// Every time an emitter states that it requires a specific property, the allocator will add that property for
    /// *all* particles, even particles managed by emitters that have no use for that property.  This adds memory usage
    /// for the benefit of re-usability of particles.
    ///
    /// This means that in general you should favor the minimum number of `ParticleAllocator` instances as
    /// possible, but if memory capacity becomes an issue than it is possible to have multiple instances based on
    /// what type of property groupings different emitters use.
    ///
    /// Particle allocation operations (and particles in general) are *not* thread safe.
    /// </summary>
    public class ParticleAllocator
    {
        /// <summary>
        /// Minimum amount capacity should grow if we run out of room 
        /// </summary>
        private const float GrowBy = 1.2f;
        
        private readonly SortedSet<Reservation> _reservations = new(new ReservationComparer());
        private Dictionary<Type, Dictionary<string, Array>> _particleProperties = new ();
        private int _capacity;
        private int _freeSpaceAvailable;

        public ParticleAllocator(int initialCapacity)
        {
            _capacity = initialCapacity;
            _freeSpaceAvailable = initialCapacity;
        }

        /// <summary>
        /// Reserves a new block of particles
        /// </summary>
        internal Reservation Reserve(int capacityRequested)
        {
            while (true)
            {
                if (capacityRequested > _freeSpaceAvailable)
                {
                    // Not enough free space, so expand the number of particles we are managing and try again
                    // Make sure we not only grow the capacity, but grow it with enough room that we won't be likely
                    // to have to immediately re-grow it next reservation
                    var growthByRequiredCapacity = (capacityRequested - _freeSpaceAvailable + _capacity) * GrowBy;
                    var newCapacity = (int) Math.Max(_capacity * GrowBy, growthByRequiredCapacity);
                    ExpandAllocatorCapacity(newCapacity);

                    continue;
                }

                // We have enough free space, so let's see if there's a gap we can fit into
                var startIndex = 0;
                var spotFound = false;
                foreach (var existingReservation in _reservations)
                {
                    var difference = existingReservation.StartIndex - startIndex;
                    if (difference >= capacityRequested)
                    {
                        spotFound = true;
                        break;
                    }

                    startIndex = existingReservation.LastUsedIndex + 1;
                }

                if (!spotFound)
                {
                    // Is there enough room at the end?
                    if (_capacity - startIndex < capacityRequested)
                    {
                        // No suitable gaps anywhere.  We have two options here, we can either defrag the current 
                        // set of reservations, or we can grow the capacity (which will also defrag as a by-product).  
                        // Defrag is a good option as it doesn't require any new allocations.  However, we also don't want
                        // to defrag if it won't leave us with much free space, as that likely means we are going to have
                        // to expand capacity anyway.
                        // TODO: Add defrag logic.

                        ExpandAllocatorCapacity((int) (_capacity * GrowBy));
                        continue;
                    }
                }

                // Since we got here, this means we found a suitable spot for this reservation.
                var reservation = new Reservation(this) {StartIndex = startIndex, LastUsedIndex = startIndex + capacityRequested - 1,};

                _reservations.Add(reservation);
                _freeSpaceAvailable -= capacityRequested;

                return reservation;
            }
        }

        /// <summary>
        /// Registers a new property that should be managed for all particles in the allocator
        /// </summary>
        internal void RegisterProperty<T>(string propertyName)
        {
            _particleProperties.TryAdd(typeof(T), new Dictionary<string, Array>());
            _particleProperties[typeof(T)].TryAdd(propertyName, new T[_capacity]);
        }

        private void ExpandReservationCapacity(Reservation reservation, int additionalRequested)
        {
            throw new NotImplementedException();
        }

        private void Release(Reservation reservation)
        {
            _reservations.Remove(reservation);
            _freeSpaceAvailable += reservation.LastUsedIndex - reservation.StartIndex + 1;
        }

        private Span<T> GetPropertyValues<T>(Reservation reservation, string property)
        {
            if (reservation == null || reservation.IsDisposed)
            {
                throw new ArgumentException("GetPropertyValues called with null or disposed reservation");
            }
            
            var dictionary = _particleProperties[typeof(T)];
            var array = (T[]) dictionary[property];

            return array.AsSpan().Slice(reservation.StartIndex, reservation.Length);
        }

        private void ExpandAllocatorCapacity(int capacityAfterExpansion)
        {
            // We need to create a whole new set of arrays for every single existing particle property,
            // then loop through each reservation and move its existing properties from the old array to the
            // new array.  This has a high chance of changing the indices of each reservation, so we need to
            // make sure we track the old and new locations until we have moved all the reservations over.
            //
            // This method assumes that `capacityAfterExpansion` is large enough to accomodate all existing reservations.

            var newLocations = new List<(int start, int lastUsedIndex)>(_reservations.Count);
            var currentIndex = 0;
            foreach (var reservation in _reservations)
            {
                var lastIndex = currentIndex + reservation.Length - 1;
                newLocations.Add((currentIndex, lastIndex));

                currentIndex = lastIndex + 1;
            }
            
            foreach (var (type, dictionary) in _particleProperties)
            foreach (var (property, oldArray) in dictionary)
            {
                var newArray = Array.CreateInstance(type, capacityAfterExpansion);
                var idx = 0;
                foreach (var reservation in _reservations)
                {
                    var (newStart, _) = newLocations[idx];
                    Array.ConstrainedCopy(oldArray, 
                        reservation.StartIndex, 
                        newArray, 
                        newStart, 
                        reservation.Length);

                    idx++;
                }

                dictionary[property] = newArray;
            }
            
            // Now that all the particle properties have been moved over we need to make sure the reservations point
            // to the correct spot in the new arrays.  We couldn't do this before without losing track of either the
            // old or new position.
            var reservationIdx = 0;
            foreach (var reservation in _reservations)
            {
                var (start, lastUsedIndex) = newLocations[reservationIdx];
                reservation.StartIndex = start;
                reservation.LastUsedIndex = lastUsedIndex;

                reservationIdx++;
            }

            var additionalCapacity = capacityAfterExpansion - _capacity;
            _freeSpaceAvailable += additionalCapacity;
            _capacity = capacityAfterExpansion;
        }

        /// <summary>
        /// Represents an active request to reserve a block of particles.  Upon being disposed it will release the
        /// block of particles for a new reservation request.
        /// </summary>
        internal class Reservation : IDisposable
        {
            private readonly ParticleAllocator _particleAllocator;
            
            public bool IsDisposed { get; private set; }
            public int StartIndex { get; set; }
            public int LastUsedIndex { get; set; }
            public int Length => LastUsedIndex - StartIndex + 1;

            public Reservation(ParticleAllocator particleAllocator)
            {
                _particleAllocator = particleAllocator;
            }

            /// <summary>
            /// Requests additional capacity for this reservation.  May cause all particles to be moved to new
            /// indices.
            /// </summary>
            /// <param name="additionalRequested">How many more particles this reservation wants</param>
            public void Expand(int additionalRequested) 
                => _particleAllocator.ExpandReservationCapacity(this, additionalRequested);

            /// <summary>
            /// Gets all values for the specified property.  The property must already be registered in order for it
            /// to exist.  If the property was registered for a type that doesn't match `T` then it will be the same
            /// as if that property was never registered.
            /// </summary>
            /// <param name="propertyName">Case sensitive name of the property to get values for</param>
            /// <typeparam name="T">The type of values the property contains</typeparam>
            /// <returns>Returns the values of the properties only for particles managed by this reservation</returns>
            /// <exception cref="KeyNotFoundException">The property name is not registered for the specified type</exception>
            public Span<T> GetPropertyValues<T>(string propertyName)
                => _particleAllocator.GetPropertyValues<T>(this, propertyName);

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    _particleAllocator.Release(this);
                    IsDisposed = true;
                }
            }
        }
        
        private class ReservationComparer : IComparer<Reservation>
        {
            public int Compare(Reservation x, Reservation y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return x.StartIndex.CompareTo(y.StartIndex);
            }
        }
    }
}