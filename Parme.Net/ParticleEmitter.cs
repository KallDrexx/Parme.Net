using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net
{
    public class ParticleEmitter : IDisposable
    {
        private readonly ParticleCollection _particleCollection;
        private readonly Dictionary<IParticleInitializer, ISet<ParticleProperty>> _initializerProperties = new();
        private readonly Dictionary<IParticleModifier, ISet<ParticleProperty>> _modifierUpdatedProperties = new();
        private readonly Dictionary<IParticleModifier, ISet<ParticleProperty>> _modifierReadableProperties = new();
        
        private readonly List<int> _newParticleIndices = new();
        
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
        
        public string? TextureFilePath { get; set; }

        public TextureSectionCoords[] TextureSections { get; set; } = Array.Empty<TextureSectionCoords>();

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
            TextureFilePath = config.TextureFilePath;
            
            Reservation = particleAllocator.Reserve(initialCapacity);
            _particleCollection = new ParticleCollection(Reservation);

            var propertiesToRegister = new HashSet<ParticleProperty>();

            foreach (var initializer in Initializers)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                var properties = initializer.PropertiesISet ?? new HashSet<ParticleProperty>();
                _initializerProperties.Add(initializer, properties);

                foreach (var property in properties)
                {
                    particleAllocator.RegisterProperty(property.Type, property.Name);
                }
            }

            foreach (var modifier in Modifiers)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                var propertiesToUpdate = modifier.PropertiesIUpdate ?? new HashSet<ParticleProperty>();
                _modifierUpdatedProperties.Add(modifier, propertiesToUpdate);
                
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
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
            var timeAliveValues = Reservation.GetPropertyValues<float>(StandardParmeProperties.TimeAlive);
            var isAlive = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive);

            for (var index = 0; index < Reservation.Length; index++)
            {
                timeAliveValues[index] += timeSinceLastFrame;
                isAlive[index] = isAlive[index] && timeAliveValues[index] <= MaxParticleLifetime;
            }
            
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

        public ParticleCollection CreateParticleCollection(
            ISet<ParticleProperty>? readableProperties,
            ISet<ParticleProperty>? settableProperties)
        {
            var collection = new ParticleCollection(Reservation)
            {
                ValidPropertiesToRead = readableProperties,
                ValidPropertiesToSet = settableProperties,
            };
            
            collection.RegisterProperties();
            
            return collection;
        }

        public bool HasAnyLiveParticles()
        {
            var isAliveValues = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive);
            for (var x = 0; x < isAliveValues.Length; x++)
            {
                if (isAliveValues[x])
                {
                    return true;
                }
            }

            return false;
        }

        public T? GetBehavior<T>() where T : class
        {
            if (typeof(IParticleInitializer).IsAssignableFrom(typeof(T)))
            {
                return Initializers.OfType<T>().FirstOrDefault();
            }

            if (typeof(IParticleModifier).IsAssignableFrom(typeof(T)))
            {
                return Modifiers.OfType<T>().FirstOrDefault();
            }

            if (Trigger is T trigger)
            {
                return trigger;
            }

            return null;
        }
        
        public void Dispose()
        {
            Reservation.Dispose();
        }

        public T? GetModifierOfType<T>() where T : class, IParticleModifier
        {
            return Modifiers
                .OfType<T>()
                .SingleOrDefault();
        }

        private void CreateNewParticles(float timeSinceLastFrame)
        {
            var particlesToCreate = Trigger.DetermineNumberOfParticlesToCreate(this, timeSinceLastFrame);
            if (particlesToCreate > 0)
            {
                _newParticleIndices.Clear();
                var particleIndex = 0;

                while (particlesToCreate > 0)
                {
                    var isAliveValues = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive);
                    var timeAliveValues = Reservation.GetPropertyValues<float>(StandardParmeProperties.TimeAlive);
                    for (; particleIndex < isAliveValues.Length; particleIndex++)
                    {
                        if (!isAliveValues[particleIndex])
                        {
                            _newParticleIndices.Add(particleIndex);
                            isAliveValues[particleIndex] = true;
                            timeAliveValues[particleIndex] = 0;
                            particlesToCreate--;

                            if (particlesToCreate <= 0)
                            {
                                break;
                            }
                        }
                    }

                    if (particlesToCreate > 0)
                    {
                        // We still need to create more particles, but we ran out of room
                        var additionalRequested = (int) Math.Ceiling(Reservation.Length * 1.5 - Reservation.Length);
                        Reservation.Expand(additionalRequested);
                    }
                }

                foreach (var initializer in Initializers)
                {
                    _initializerProperties.TryGetValue(initializer, out var properties);
                    _particleCollection.ValidPropertiesToSet = properties;
                    _particleCollection.ValidPropertiesToRead = null;
                    
                    initializer.InitializeParticles(this, _particleCollection, _newParticleIndices);
                }

                var positionX = Reservation.GetPropertyValues<float>(StandardParmeProperties.PositionX);
                var positionY = Reservation.GetPropertyValues<float>(StandardParmeProperties.PositionY);
                foreach (var index in _newParticleIndices)
                {
                    positionX[index] += WorldCoordinates.X;
                    positionY[index] += WorldCoordinates.Y;
                }
            }
        }
    }
}