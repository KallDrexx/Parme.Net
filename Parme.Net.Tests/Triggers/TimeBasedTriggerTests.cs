using System;
using Parme.Net.Triggers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Triggers
{
    public class TimeBasedTriggerTests
    {
        [Fact]
        public void No_Particles_Emitted_Until_Time_Limit_Hit()
        {
            var trigger = new TimeBasedTrigger(new Random())
            {
                SecondsBetweenEmissions = 1.0f,
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
            
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.90f).ShouldBe(0);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.05f).ShouldBe(0);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.06f).ShouldBeInRange(5, 10);
        }

        [Fact]
        public void Timer_Reset_After_Particles_Emitted()
        {
            var trigger = new TimeBasedTrigger(new Random())
            {
                SecondsBetweenEmissions = 1.0f,
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
            
            trigger.DetermineNumberOfParticlesToCreate(emitter, 1.05f).ShouldBeInRange(5, 10);
            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f).ShouldBe(0);
        }

        [Fact]
        public void Only_One_Set_Of_Particles_Emitted_If_Multiples_Of_Time_Elapses()
        {
            var trigger = new TimeBasedTrigger(new Random())
            {
                SecondsBetweenEmissions = 1.0f,
                MinParticlesToEmit = 5,
                MaxParticlesToEmit = 5,
            };

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger,
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);
            
            trigger.DetermineNumberOfParticlesToCreate(emitter, 5.0f).ShouldBe(5);
        }
    }
}