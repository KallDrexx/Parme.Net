using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;

namespace Parme.Net
{
    public class Emitter
    {
        private readonly ParticleAllocator.Reservation _reservation;
        private readonly ParticleCollection _particleCollection;
        private readonly Dictionary<ParticleBehavior, HashSet<ParticleProperty>> _initializedProperties = new();
        private readonly Dictionary<ParticleBehavior, HashSet<ParticleProperty>> _modifiedProperties = new();
        private readonly List<int> _newParticleIndices = new();
        
        /// <summary>
        /// The set of behaviors this emitter is using.  Behaviors will be executed in the order they are passed in by
        /// </summary>
        public IReadOnlyList<ParticleBehavior> Behaviors { get; }
        
        /// <summary>
        /// Defines how new particles are created
        /// </summary>
        public ParticleTrigger Trigger { get; } 
        
        /// <summary>
        /// Where the emitter is in world space.
        /// </summary>
        public Vector2 WorldCoordinates { get; set; }
        
        /// <summary>
        /// Determines if the emitter is actively creating new particles or not
        /// </summary>
        public bool IsEmittingNewParticles { get; set; }

        public Emitter(ParticleAllocator particleAllocator,
            ParticleTrigger trigger,
            IEnumerable<ParticleBehavior> behaviors, 
            int? initialCapacity = null)
        {
            initialCapacity ??= 50; // TODO: attempt to estimate based on behaviors

            Trigger = trigger;
            Behaviors = new List<ParticleBehavior>(behaviors);
            _reservation = particleAllocator.Reserve(initialCapacity.Value);
            _particleCollection = new ParticleCollection(_reservation);

            foreach (var behavior in Behaviors)
            {
                var initializedProperties = behavior.InitializedProperties;
                if (initializedProperties.Any())
                {
                    _initializedProperties.Add(behavior, initializedProperties);
                }
                
                var modifiedProperties = behavior.ModifiedProperties;
                if (modifiedProperties.Any())
                {
                    _modifiedProperties.Add(behavior, modifiedProperties);
                }

                foreach (var property in initializedProperties.Concat(modifiedProperties))
                {
                    particleAllocator.RegisterProperty(property.Type, property.Name);
                }
            }

            var standardProperties = new[]
            {
                StandardParmeProperties.IsAlive,
                StandardParmeProperties.TimeAlive
            };

            foreach (var property in standardProperties)
            {
                particleAllocator.RegisterProperty(property.Type, property.Name);
            }
        }

        public void Update(float timeSinceLastFrame)
        {
            foreach (var (behavior, properties) in _modifiedProperties)
            {
                _particleCollection.ValidProperties = properties;
                behavior.UpdateParticles(this, _particleCollection, timeSinceLastFrame);
            }

            if (IsEmittingNewParticles)
            {
                var particlesToCreate = Trigger.DetermineNumberOfParticlesToCreate(this, timeSinceLastFrame);
                if (particlesToCreate > 0)
                {
                    _newParticleIndices.Clear();
                    var particleIndex = 0;

                    while (particlesToCreate > 0)
                    {
                        var isAliveValues = _reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
                        for (; particleIndex < isAliveValues.Length; particleIndex++)
                        {
                            if (!isAliveValues[particleIndex])
                            {
                                _newParticleIndices.Add(particleIndex);
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
                            var newSize = (int) Math.Ceiling(_reservation.Length * 1.5 - _reservation.Length);
                            _reservation.Expand(newSize);
                        }
                    }
                    
                    foreach (var (behavior, properties) in _initializedProperties)
                    {
                        _particleCollection.ValidProperties = properties;
                        behavior.InitializeCreatedParticles(this, _particleCollection, _newParticleIndices);
                    }
                }
            }
        }
    }
}