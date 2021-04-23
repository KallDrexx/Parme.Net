using System;
using System.Linq;
using System.Numerics;
using Parme.Net.Behaviors;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Behaviors
{
    public class SizingBehaviorTests : TestBase
    {
        [Fact]
        public void Current_And_Initial_Height_And_Width_Zeroed_Out_On_Initialization()
        {
            var trigger = MockTrigger();
            var behavior = new SizingBehavior(new Random())
            {
                MinParticleInitialSize = new Vector2(3, 4),
                MaxParticleInitialSize = new Vector2(3, 4),
            };
            
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig {Trigger = trigger.Object, Behaviors = {behavior}};
            var emitter = new ParticleEmitter(allocator, config);
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidProperties = behavior.InitializedProperties,
            };

            var currentHeight = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            var initialHeight = collection.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = collection.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            
            for (var x = 0; x < collection.Count; x++)
            {
                currentHeight[x] = 1f;
                currentWidth[x] = 2f;
                initialHeight[x] = 5f;
                initialWidth[x] = 6f;
            }

            var indices = new[] {0, 3, 5, 7};
            behavior.InitializeCreatedParticles(emitter, collection, indices);

            foreach (var x in indices)
            {
                currentWidth[x].ShouldBe(3);
                currentHeight[x].ShouldBe(4);
                initialWidth[x].ShouldBe(3);
                initialHeight[x].ShouldBe(4);
            }
        }

        [Fact]
        public void Current_And_Initial_Height_And_Width_Should_Not_Be_Touched_On_Initialization_For_Non_New_Particles()
        {
            var trigger = MockTrigger();
            var behavior = new SizingBehavior(new Random())
            {
                MinParticleInitialSize = new Vector2(3, 4),
                MaxParticleInitialSize = new Vector2(3, 4),
            };
            
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig {Trigger = trigger.Object, Behaviors = {behavior}};
            var emitter = new ParticleEmitter(allocator, config);
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidProperties = behavior.InitializedProperties,
            };

            var currentHeight = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            var initialHeight = collection.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = collection.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            
            for (var x = 0; x < collection.Count; x++)
            {
                currentHeight[x] = 1f;
                currentWidth[x] = 2f;
                initialHeight[x] = 5f;
                initialWidth[x] = 6f;
            }

            var indices = new[] {0, 3, 5, 7};
            behavior.InitializeCreatedParticles(emitter, collection, indices);

            foreach (var x in Enumerable.Range(0, 10).Except(indices))
            {
                currentHeight[x].ShouldBe(1f);
                currentWidth[x].ShouldBe(2f);
                initialHeight[x].ShouldBe(5f);
                initialWidth[x].ShouldBe(6f);
            }
        }

        [Fact]
        public void Size_Performs_Linear_Interpolation_Based_On_Initial_And_Ending_Size()
        {
            var trigger = MockTrigger();
            var behavior = new SizingBehavior(new Random())
            {
                MinParticleInitialSize = new Vector2(3, 4),
                MaxParticleInitialSize = new Vector2(3, 4),
                EndingSize = new Vector2(10, 20),
            };
            
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig {Trigger = trigger.Object, Behaviors = {behavior}, MaxParticleLifetime = 5};
            var emitter = new ParticleEmitter(allocator, config);
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidProperties = behavior.ModifiedProperties,
            };

            var currentHeight = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            var initialHeight = collection.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = collection.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            var timeAlive = collection.GetPropertyValues<float>(StandardParmeProperties.TimeAlive.Name);

            for (var x = 0; x < collection.Count; x++)
            {
                currentHeight[x] = 1f;
                currentWidth[x] = 2f;
                initialHeight[x] = behavior.MinParticleInitialSize.Y;
                initialWidth[x] = behavior.MinParticleInitialSize.X;
                timeAlive[x] = 3.5f;
            }
            
            behavior.UpdateParticles(emitter, collection, 0.16f);

            var expectedWidth = behavior.MinParticleInitialSize.X +
                                (3.5f / config.MaxParticleLifetime) *
                                (behavior.EndingSize.X - behavior.MinParticleInitialSize.X);
            
            var expectedHeight = behavior.MinParticleInitialSize.Y +
                                (3.5f / config.MaxParticleLifetime) *
                                (behavior.EndingSize.Y - behavior.MinParticleInitialSize.Y);
            
            for (var x = 0; x < collection.Count; x++)
            {
                currentHeight[x].ShouldBe(expectedHeight);
                currentWidth[x].ShouldBe(expectedWidth);
            }
        }
    }
}