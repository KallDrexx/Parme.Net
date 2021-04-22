using System;
using System.Numerics;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Triggers
{
    public class DistanceBasedTriggerTests
    {
        [Fact]
        public void Particles_Only_Emitted_After_Reaching_Distance_Threshold()
        {
            var trigger = new DistanceBasedTrigger(new Random())
            {
                DistanceBetweenEmissions = 1.0f,
                MinParticlesToEmit = 5,
                MaxParticlesToEmit = 10,
            };
            
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger,
                InitialCapacity = 10,
            };
            var emitter = new ParticleEmitter(allocator, config);

            emitter.WorldCoordinates = Vector2.Zero;
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);

            emitter.WorldCoordinates = new Vector2(0.5f, 0.5f);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);

            emitter.WorldCoordinates = new Vector2(1, 1);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBeInRange(5, 10);
        }

        [Fact]
        public void Tracked_Distance_Reset_After_Emission()
        {
            var trigger = new DistanceBasedTrigger(new Random())
            {
                DistanceBetweenEmissions = 1.0f,
                MinParticlesToEmit = 5,
                MaxParticlesToEmit = 10,
            };
            
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger,
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            emitter.WorldCoordinates = Vector2.Zero;
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);

            emitter.WorldCoordinates = new Vector2(0.5f, 0.5f);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);

            emitter.WorldCoordinates = new Vector2(1, 1);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBeInRange(5, 10);

            emitter.WorldCoordinates = new Vector2(1, 1.5f);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);
        }
    }
}