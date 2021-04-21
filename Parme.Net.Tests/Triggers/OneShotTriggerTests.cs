using System;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Triggers
{
    public class OneShotTriggerTests
    {
        [Fact]
        public void Returns_Value_Between_Min_And_Max_On_Invocation()
        {
            const int seed = 1;
            var trigger = new OneShotTrigger(new Random(seed))
            {
                MinParticlesToEmit = 5,
                MaxParticlesToEmit = 10,
            };

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger, ArraySegment<IParticleBehavior>.Empty);

            var result = trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f);
            
            result.ShouldBe(new Random(seed).Next(trigger.MinParticlesToEmit, trigger.MaxParticlesToEmit + 1));
        }

        [Fact]
        public void Turns_Emitter_Off_After_Trigger_Runs()
        {
            var trigger = new OneShotTrigger(new Random())
            {
                MinParticlesToEmit = 5,
                MaxParticlesToEmit = 10,
            };

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger, ArraySegment<IParticleBehavior>.Empty)
            {
                IsEmittingNewParticles = true,
            };

            trigger.DetermineNumberOfParticlesToCreate(emitter, 0.16f);
            
            emitter.IsEmittingNewParticles.ShouldBeFalse();
        }
    }
}