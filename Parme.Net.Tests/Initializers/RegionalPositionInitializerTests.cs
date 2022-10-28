using System;
using System.Linq;
using System.Numerics;
using Parme.Net.Initializers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Initializers
{
    public class RegionalPositionInitializerTests : TestBase
    {
        [Fact]
        public void Can_Be_Cloned()
        {
            var initializer = new RegionalPositionInitializer(new Random())
            {
                MinRelativePosition = new Vector2(3, 4),
                MaxRelativePosition = new Vector2(5, 6),
            };

            var cloned = (RegionalPositionInitializer) initializer.Clone();
            cloned.MinRelativePosition.ShouldBe(new Vector2(3, 4));
            cloned.MaxRelativePosition.ShouldBe(new Vector2(5, 6));
        }
        
        [Fact]
        public void Sets_Initialized_Position_For_New_Particles()
        {
            var initializer = new RegionalPositionInitializer(new Random())
            {
                MinRelativePosition = new Vector2(3, 4),
                MaxRelativePosition = new Vector2(3, 4),
            };

            var (collection, newIndices) = RunInitializer(initializer);
            
            var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX);
            var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY);
            foreach (var index in newIndices)
            {
                positionX[index].ShouldBe(3);
                positionY[index].ShouldBe(4);
            }
        }

        [Fact]
        public void Non_New_Particles_Do_Not_Have_Their_Position_Set()
        {
            var initializer = new RegionalPositionInitializer(new Random())
            {
                MinRelativePosition = new Vector2(3, 4),
                MaxRelativePosition = new Vector2(3, 4),
            };
            
            var (collection, newIndices) = RunInitializer(initializer);
            var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX);
            var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY);
            foreach (var index in Enumerable.Range(0, positionX.Length).Except(newIndices))
            {
                positionX[index].ShouldBe(0);
                positionY[index].ShouldBe(0);
            }
        }
    }
}