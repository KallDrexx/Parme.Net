using System.Collections.Generic;
using Moq;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests
{
    public class ParticleEmitterTests : TestBase
    {
        [Fact]
        public void Mandatory_Properties_Are_Automatically_Registered()
        {
            var trigger = MockTrigger();

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            var reservation = emitter.Reservation;
            reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.TimeAlive.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.CurrentWidth.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.CurrentHeight.Name);
        }
        
        [Fact]
        public void Trigger_Called_If_Emitter_Is_Active()
        {
            var trigger = MockTrigger();

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config)
            {
                IsEmittingNewParticles = true,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Once);
        }

        [Fact]
        public void Trigger_Not_Called_If_Emitter_Is_Not_Active()
        {
            var trigger = MockTrigger();

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config)
            {
                IsEmittingNewParticles = false,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Never);
        }

        [Fact]
        public void Initializer_Properties_Are_Registered()
        {
            var trigger = MockTrigger();
            var initializer = MockInitializer();
            initializer.Setup(x => x.PropertiesISet)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Initializers = { initializer.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            emitter.Reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Modifier_Properties_To_Update_Are_Registered()
        {
            var trigger = MockTrigger();
            var modifier = MockModifier();
            modifier.Setup(x => x.PropertiesIUpdate)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Modifiers = { modifier.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);
            emitter.Reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Modifier_Properties_To_Read_Are_Registered()
        {
            var trigger = MockTrigger();
            var modifier = MockModifier();
            modifier.Setup(x => x.PropertiesIUpdate)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Modifiers = { modifier.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);
            emitter.Reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Behavior_Initialization_Method_Called_When_Trigger_Returns_A_Positive_Number()
        {
            const int newParticleCount = 5;
            
            var trigger = MockTrigger();
            var initializer = MockInitializer();
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Initializers = { initializer.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);
            
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f))
                .Returns(newParticleCount);
            
            emitter.Update(0.16f);
            
            initializer.Verify(x => 
                x.InitializeParticles(
                    emitter, 
                    It.IsAny<ParticleCollection>(), 0, newParticleCount - 1),
                Times.Once);
        }

        [Fact]
        public void Modifier_Method_Called()
        {
            var trigger = MockTrigger();
            var modifier = MockModifier();
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Modifiers = { modifier.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            emitter.Update(0.16f);
            modifier.Verify(x => x.Update(emitter, It.IsAny<ParticleCollection>(), 0.16f), Times.Once);
        }

        [Fact]
        public void Initializer_Given_New_Particle_Indices_That_Were_Previously_Dead()
        {
            const int newParticleCount = 3;
            
            var trigger = MockTrigger();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var initializer = MockInitializer();
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Initializers = { initializer.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            values[1] = true;
            values[3] = true;
            values[5] = true;
            values[7] = true;
            values[9] = true;
            
            IReadOnlyList<int>? newParticleIndices = null;
            initializer.Setup(x =>
                    x.InitializeParticles(
                        It.IsAny<ParticleEmitter>(),
                        It.IsAny<ParticleCollection>(), TODO, TODO))
                .Callback<ParticleEmitter, ParticleCollection, IReadOnlyList<int>>((_, _, indices) => newParticleIndices = indices);
            
            emitter.Update(0.16f);

            newParticleIndices.ShouldNotBeNull();
            newParticleIndices.Count.ShouldBe(3);
            newParticleIndices[0].ShouldBe(0);
            newParticleIndices[1].ShouldBe(2);
            newParticleIndices[2].ShouldBe(4);
        }

        [Fact]
        public void Emitter_Capacity_Expanded_If_Not_Enough_Dead_Particles_Exist()
        {
            const int newParticleCount = 3;
            
            var trigger = MockTrigger();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var initializer = MockInitializer();
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Initializers = { initializer.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            for (var x = 0; x <= 10 - newParticleCount; x++)
            {
                values[x] = true;
            }
            
            emitter.Update(0.16f);
            emitter.Reservation.Length.ShouldBeGreaterThan(10);
        }

        [Fact]
        public void Particles_For_Initialization_Are_Marked_As_Alive()
        {
            const int newParticleCount = 3;
            
            var trigger = MockTrigger();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var initializer = MockInitializer();
            var allocator = new ParticleAllocator(10);
            var config = new EmitterConfig
            {
                Trigger = trigger.Object,
                Initializers = { initializer.Object },
                InitialCapacity = 10,
            };
            
            var emitter = new ParticleEmitter(allocator, config);

            {
                var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
                values[1] = true;
                values[3] = true;
                values[5] = true;
                values[7] = true;
                values[9] = true;
            }

            IReadOnlyList<int>? newParticleIndices = null;
            initializer.Setup(x =>
                    x.InitializeParticles(
                        It.IsAny<ParticleEmitter>(),
                        It.IsAny<ParticleCollection>(), TODO, TODO))
                .Callback<ParticleEmitter, ParticleCollection, IReadOnlyList<int>>((_, _, indices) => newParticleIndices = indices);
            
            emitter.Update(0.16f);

            newParticleIndices.ShouldNotBeNull();
            newParticleIndices.Count.ShouldBe(3);

            {
                var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
                values[newParticleIndices[0]].ShouldBeTrue();
                values[newParticleIndices[1]].ShouldBeTrue();
                values[newParticleIndices[2]].ShouldBeTrue();
            }
        }
    }
}