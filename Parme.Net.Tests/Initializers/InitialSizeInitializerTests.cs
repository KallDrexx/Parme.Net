using System;
using System.Numerics;
using Parme.Net.Initializers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Initializers
{
    public class InitialSizeInitializerTests : TestBase
    {
        [Fact]
        public void Can_Be_Cloned()
        {
            var initializer = new InitialSizeInitializer(new Random())
            {
                MinSize = new Vector2(0, 1),
                MaxSize = new Vector2(2, 3),
            };

            var cloned = (InitialSizeInitializer) initializer.Clone();

            cloned.MinSize = initializer.MinSize;
            cloned.MaxSize = initializer.MaxSize;
        }

        [Fact]
        public void Sets_Initial_Size_Values()
        {
            var initializer = new InitialSizeInitializer(new Random())
            {
                MinSize = new Vector2(10, 20),
                MaxSize = new Vector2(15, 25),
            };
            
            var config = new EmitterConfig
            {
                Initializers = {initializer},
                Trigger = MockTrigger().Object,
                InitialCapacity = 10,
            };

            var allocator = new ParticleAllocator(100);
            var emitter = new ParticleEmitter(allocator, config);
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidPropertiesToSet = initializer.PropertiesISet,
            };
            
            var newIndices = new[] {1, 3, 5, 7};
            initializer.InitializeParticles(emitter, collection, newIndices);

            var initialHeight = collection.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = collection.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            var currentHeight = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            foreach (var index in newIndices)
            {
                initialHeight[index].ShouldBeInRange(20, 25);
                initialWidth[index].ShouldBeInRange(10, 15);
                currentHeight[index].ShouldBe(initialHeight[index]);
                currentWidth[index].ShouldBe(initialWidth[index]);
            }
        }
    }
}