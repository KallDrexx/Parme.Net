using System;
using System.Linq;
using System.Numerics;
using Parme.Net.Initializers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Initializers
{
    public class SizeInitializerTests : TestBase
    {
        [Fact]
        public void Can_Be_Cloned()
        {
            var initializer = new SizeInitializer(new Random())
            {
                MinSize = new Vector2(0, 1),
                MaxSize = new Vector2(2, 3),
            };

            var cloned = (SizeInitializer) initializer.Clone();

            cloned.MinSize = initializer.MinSize;
            cloned.MaxSize = initializer.MaxSize;
        }

        [Fact]
        public void Sets_Initial_Size_Values()
        {
            var initializer = new SizeInitializer(new Random())
            {
                MinSize = new Vector2(10, 20),
                MaxSize = new Vector2(15, 25),
            };

            var (collection, newIndices) = RunInitializer(initializer);

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

        [Fact]
        public void Non_New_Particles_Do_Not_Have_Values_Set()
        {
            var initializer = new SizeInitializer(new Random())
            {
                MinSize = new Vector2(10, 20),
                MaxSize = new Vector2(15, 25),
            };
            
            var (collection, newIndices) = RunInitializer(initializer);

            var initialHeight = collection.GetPropertyValues<float>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = collection.GetPropertyValues<float>(StandardParmeProperties.InitialWidth.Name);
            var currentHeight = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = collection.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            foreach (var index in Enumerable.Range(0, collection.Count).Where(x => !newIndices.Contains(x)))
            {
                initialHeight[index].ShouldBe(0);
                initialWidth[index].ShouldBe(0);
                currentHeight[index].ShouldBe(0);
                currentWidth[index].ShouldBe(0);
            }
        }
    }
}