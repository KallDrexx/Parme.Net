using System;
using System.Collections.Generic;
using System.Numerics;
using Moq;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Behaviors
{
    public class InitialPositionBehaviorTests
    {
        [Fact]
        public void Behavior_Initialization_Assigns_Position_Relative_To_Emitter()
        {
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new InitialPositionBehavior(new Random())
            {
                MinPositionRelativeToEmitter = new Vector2(2, 3),
                MaxPositionRelativeToEmitter = new Vector2(2, 3),
            };

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Behaviors = { behavior },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config)
            {
                WorldCoordinates = new Vector2(5, 7),
            };
            
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidProperties = behavior.InitializedProperties,
            };

            var indicesToUpdate = new[] {1, 2, 4};
            behavior.InitializeCreatedParticles(emitter, collection, indicesToUpdate);

            var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
            
            foreach (var index in indicesToUpdate)
            {
                positionX[index].ShouldBe(5 + 2);
                positionY[index].ShouldBe(7 + 3);
            }
        }

        [Fact]
        public void Behavior_initialization_Does_Not_Assign_To_Non_New_Particles()
        {
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new InitialPositionBehavior(new Random())
            {
                MinPositionRelativeToEmitter = new Vector2(2, 3),
                MaxPositionRelativeToEmitter = new Vector2(2, 3),
            };

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Behaviors = { behavior },
                InitialCapacity = 5,
            };
            var emitter = new ParticleEmitter(allocator, config)
            {
                WorldCoordinates = new Vector2(5, 7),
            };
            
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidProperties = behavior.InitializedProperties,
            };

            var indicesToUpdate = new List<int>{1, 2, 4};
            behavior.InitializeCreatedParticles(emitter, collection, indicesToUpdate);

            var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);

            for (var index = 0; index < collection.Count; index++)
            {
                if (indicesToUpdate.Contains(index))
                {
                    continue;
                }
                
                positionX[index].ShouldBe(0);
                positionY[index].ShouldBe(0);
            }
        }

        [Fact]
        public void Can_Clone_Behavior()
        {
            var behavior = new InitialPositionBehavior(new Random())
            {
                MinPositionRelativeToEmitter = new Vector2(2, 3),
                MaxPositionRelativeToEmitter = new Vector2(2, 3),
            };

            var clone = (InitialPositionBehavior) behavior.Clone();

            clone.ShouldNotBeNull();
            clone.MinPositionRelativeToEmitter.ShouldBe(behavior.MinPositionRelativeToEmitter);
            clone.MaxPositionRelativeToEmitter.ShouldBe(behavior.MaxPositionRelativeToEmitter);
        }
    }
}