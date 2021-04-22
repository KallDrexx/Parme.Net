using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;

namespace Parme.Net
{
    public class ParticleEmitter
    {
        private readonly ParticleCollection _particleCollection;
        private readonly Dictionary<ParticleBehavior, HashSet<ParticleProperty>> _initializedProperties = new();
        private readonly Dictionary<ParticleBehavior, HashSet<ParticleProperty>> _modifiedProperties = new();
        private readonly List<int> _newParticleIndices = new();
        
        internal ParticleAllocator.Reservation Reservation { get; } // internal for test simplification 
        
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
        public bool IsEmittingNewParticles { get; set; } = true;

        public ParticleEmitter(ParticleAllocator particleAllocator, EmitterConfig config)
        {
            var initialCapacity = config?.InitialCapacity ?? 50; // TODO: attempt to estimate based on behaviors and triggers

            if (config?.Trigger == null)
            {
                throw new ArgumentException("Emitter config did not have a trigger, but one is required");
            }

            Trigger = config.Trigger.Clone();
            Behaviors = config.Behaviors.Select(x => x.Clone()).ToArray();
            Reservation = particleAllocator.Reserve(initialCapacity);
            _particleCollection = new ParticleCollection(Reservation);

            foreach (var behavior in Behaviors)
            {
                // ReSharper disable once ConstantNullCoalescingCondition (due to consumer code without nullable refs)
                var initializedProperties = behavior.InitializedProperties ?? new HashSet<ParticleProperty>();
                if (initializedProperties.Any())
                {
                    _initializedProperties.Add(behavior, initializedProperties);
                }
                
                // ReSharper disable once ConstantNullCoalescingCondition (due to consumer code without nullable refs)
                var modifiedProperties = behavior.ModifiedProperties ?? new HashSet<ParticleProperty>();
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
            foreach (var (behavior, properties) in _modifiedProperties)
            {
                _particleCollection.ValidProperties = properties;
                behavior.UpdateParticles(this, _particleCollection, timeSinceLastFrame);
            }

            if (IsEmittingNewParticles)
            {
                CreateNewParticles(timeSinceLastFrame);
            }
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
                    var isAliveValues = Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
                    for (; particleIndex < isAliveValues.Length; particleIndex++)
                    {
                        if (!isAliveValues[particleIndex])
                        {
                            _newParticleIndices.Add(particleIndex);
                            isAliveValues[particleIndex] = true;
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

                foreach (var (behavior, properties) in _initializedProperties)
                {
                    _particleCollection.ValidProperties = properties;
                    behavior.InitializeCreatedParticles(this, _particleCollection, _newParticleIndices);
                }
            }
        }
    }
}