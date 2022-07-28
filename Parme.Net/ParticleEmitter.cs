using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net
{
    public class ParticleEmitter
    {
        private readonly ParticleCollection _particleCollection;
        private readonly Dictionary<IParticleInitializer, IReadOnlySet<ParticleProperty>> _initializerProperties = new();
        private readonly Dictionary<IParticleModifier, IReadOnlySet<ParticleProperty>> _modifierUpdatedProperties = new();
        private readonly Dictionary<IParticleModifier, IReadOnlySet<ParticleProperty>> _modifierReadableProperties = new();

        private bool _hasAnyLiveParticles;
        private int _firstAliveIndex;
        private int _lastAliveIndex;
        
        internal ParticleAllocator.Reservation Reservation { get; } // internal for test purposes 
        
        /// <summary>
        /// Defines how new particles are created
        /// </summary>
        public ParticleTrigger Trigger { get; } 
        
        /// <summary>
        /// The set of initializers the emitter uses.  Initializers will be executed in the order they appear
        /// </summary>
        public IReadOnlyList<IParticleInitializer> Initializers { get; }
        
        /// <summary>
        /// The set of modifiers the emitter uses.  Modifiers will be executed in collection order
        /// </summary>
        public IReadOnlyList<IParticleModifier> Modifiers { get; }
        
        /// <summary>
        /// Where the emitter is in world space.  This will cause new particles to start relative to these X and Y
        /// values.
        /// </summary>
        public Vector2 WorldCoordinates { get; set; }
        
        /// <summary>
        /// The angle in which the emitter is currently rotated by.  Newly emitted particles will start with a
        /// rotation relative to this angle.
        /// </summary>
        public float RotationInRadians { get; set; }

        /// <summary>
        /// Determines if the emitter is actively creating new particles or not
        /// </summary>
        public bool IsEmittingNewParticles { get; set; } = true;
        
        /// <summary>
        /// How many seconds a single particle is expected to be alive for
        /// </summary>
        public float MaxParticleLifetime { get; set; }

        public ParticleEmitter(ParticleAllocator particleAllocator, EmitterConfig config)
        {
            var initialCapacity = config?.InitialCapacity ?? 50; // TODO: attempt to estimate based on behaviors and triggers

            if (config?.Trigger == null)
            {
                throw new ArgumentException("Emitter config did not have a trigger, but one is required");
            }

            Trigger = config.Trigger.Clone();
            Initializers = config.Initializers.Select(x => x.Clone()).ToArray();
            Modifiers = config.Modifiers.Select(x => x.Clone()).ToArray();
            MaxParticleLifetime = config.MaxParticleLifetime;
            
            Reservation = particleAllocator.Reserve(initialCapacity);
            _particleCollection = new ParticleCollection(Reservation);

            foreach (var initializer in Initializers)
            {
                // ReSharper disable once ConstantNullCoalescingCondition (due to consumer code without nullable refs)
                var properties = initializer.PropertiesISet ?? new HashSet<ParticleProperty>();
                _initializerProperties.Add(initializer, properties);

                foreach (var property in properties)
                {
                    particleAllocator.RegisterProperty(property.Type, property.Name);
                }
            }

            foreach (var modifier in Modifiers)
            {
                // ReSharper disable once ConstantNullCoalescingCondition
                var propertiesToUpdate = modifier.PropertiesIUpdate ?? new HashSet<ParticleProperty>();
                _modifierUpdatedProperties.Add(modifier, propertiesToUpdate);
                
                // ReSharper disable once ConstantNullCoalescingCondition
                var propertiesToRead = modifier.PropertiesIRead ?? new HashSet<ParticleProperty>();
                _modifierReadableProperties.Add(modifier, propertiesToRead);

                foreach (var property in propertiesToUpdate.Concat(propertiesToRead))
                {
                    particleAllocator.RegisterProperty(property.Type, property.Name);
                }
            }

            var standardProperties = new[]
            {
                StandardParmeProperties.IsAlive,
                StandardParmeProperties.TimeAlive,
                StandardParmeProperties.PositionX,
                StandardParmeProperties.PositionY,
                StandardParmeProperties.CurrentHeight,
                StandardParmeProperties.CurrentWidth,
            };

            foreach (var property in standardProperties)
            {
                particleAllocator.RegisterProperty(property.Type, property.Name);
            }
        }

        public void Update(float timeSinceLastFrame)
        {
            foreach (var modifier in Modifiers)
            {
                _modifierReadableProperties.TryGetValue(modifier, out var readableProperties);
                _modifierUpdatedProperties.TryGetValue(modifier, out var updateableProperties);

                _particleCollection.ValidPropertiesToRead = readableProperties;
                _particleCollection.ValidPropertiesToSet = updateableProperties;
                
                modifier.Update(this, _particleCollection, timeSinceLastFrame);
            }

            if (IsEmittingNewParticles)
            {
                CreateNewParticles(timeSinceLastFrame);
            }
        }

        private void CreateNewParticles(float timeSinceLastFrame)
        {
            var particlesToCreate = Trigger.DetermineNumberOfParticlesToCreate(this, timeSinceLastFrame);
            if (particlesToCreate == 0)
            {
                return;
            }

            var firstNewIndex = GetFirstNewParticleIndex(particlesToCreate);
            var isAliveValues = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            
            // First set all new particles to alive.  
            for (var count = 0; count < particlesToCreate; count++)
            {
                isAliveValues[firstNewIndex + count] = true;
            }
            
            foreach (var initializer in Initializers)
            {
                _initializerProperties.TryGetValue(initializer, out var properties);
                _particleCollection.ValidPropertiesToSet = properties;
                _particleCollection.ValidPropertiesToRead = null;
            
                initializer.InitializeParticles(
                    this, 
                    _particleCollection, 
                    firstNewIndex,
                    firstNewIndex + particlesToCreate - 1);
            }
        }

        private void UpdateLiveParticleBounds()
        {
            if (!_hasAnyLiveParticles)
            {
                // Nothing to update since we shouldn't have been any particles since we know they aren't alive
                return;
            }
            
            var isAliveValues = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);

            var foundLiveParticle = false;
            for (var index = _lastAliveIndex; index <= _lastAliveIndex; index++)
            {
                if (isAliveValues[index])
                {
                    _firstAliveIndex = index;
                    foundLiveParticle = true;
                    break;
                }
            }

            if (!foundLiveParticle)
            {
                _hasAnyLiveParticles = false;
                _firstAliveIndex = 0;
                _lastAliveIndex = 0;

                return;
            }
            
            // We definitely have at least one live particle, so bring in the tail end
            for (var index = _lastAliveIndex; index > _firstAliveIndex; index--)
            {
                if (isAliveValues[index])
                {
                    _lastAliveIndex = index;
                    return;
                }
            }
        }

        private int GetFirstNewParticleIndex(int amountToAdd)
        {
            UpdateLiveParticleBounds();
            
            if (!_hasAnyLiveParticles)
            {
                if (_particleCollection.Count < amountToAdd)
                {
                    GrowWithMinimum(amountToAdd);
                }

                return 0;
            }
            
            // Do we have enough space at the tail end?
            if (_lastAliveIndex + 1 + amountToAdd <= _particleCollection.Count)
            {
                return _lastAliveIndex + 1;
            }
            
            // do we have enough space at the head?
            if (_firstAliveIndex <= amountToAdd)
            {
                return _firstAliveIndex - amountToAdd;
            }
            
            // Not enough contiguous room at either end, so grow the collection
            GrowWithMinimum(amountToAdd);
            
            // Since our particles were shuffled around, we need to find the new bounds for them
            _firstAliveIndex = 0;
            _lastAliveIndex = _particleCollection.Count - 1;
            UpdateLiveParticleBounds();

            return GetFirstNewParticleIndex(amountToAdd);
        }

        private void GrowWithMinimum(int minimumToAdd)
        {
            const float GrowthFactor = 1.2f;

            var growByGrowthFactor = (int)Math.Ceiling(_particleCollection.Count * GrowthFactor);
            var growBy = Math.Max(growByGrowthFactor, minimumToAdd);
            
            _particleCollection.Expand(growBy);
        }
    }
}