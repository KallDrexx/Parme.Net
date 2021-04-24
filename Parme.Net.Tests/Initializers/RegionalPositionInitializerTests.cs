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
        public void Sets_Initialized_Position_For_New_Particles_Based_On_Emitter_Coordinates()
        {
            var initializer = new RegionalPositionInitializer(new Random())
            {
                MinRelativePosition = new Vector2(3, 4),
                MaxRelativePosition = new Vector2(3, 4),
            };
            
            var config = new EmitterConfig
            {
                Initializers = {initializer},
                Trigger = MockTrigger().Object,
                InitialCapacity = 10,
            };

            var allocator = new ParticleAllocator(100);
            var emitter = new ParticleEmitter(allocator, config)
            {
                WorldCoordinates = new Vector2(10, 11),
            };
            
            var collection = new ParticleCollection(emitter.Reservation)
            {
                ValidPropertiesToSet = initializer.PropertiesISet,
            };
            
            var newIndices = new[] {1, 3, 5, 7};
            initializer.InitializeParticles(emitter, collection, newIndices);

            var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
            foreach (var index in newIndices)
            {
                positionX[index].ShouldBe(13);
                positionY[index].ShouldBe(15);
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

            {
                var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
                var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
                for (var index = 0; index < positionX.Length; index++)
                {
                    positionX[index] = 9.9f;
                    positionY[index] = 8.8f;
                }
            }
            
            var newIndices = new[] {1, 3, 5, 7};
            initializer.InitializeParticles(emitter, collection, newIndices);

            {
                var positionX = collection.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
                var positionY = collection.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
                foreach (var index in Enumerable.Range(0, positionX.Length).Except(newIndices))
                {
                    positionX[index].ShouldBe(9.9f);
                    positionY[index].ShouldBe(8.8f);
                }
            }
        }
    }
}