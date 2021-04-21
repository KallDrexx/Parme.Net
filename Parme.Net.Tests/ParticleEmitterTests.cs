using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Moq;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests
{
    public class ParticleEmitterTests
    {
        [Fact]
        public void Basic_Properties_Are_Automatically_Registered()
        {
            var trigger = new Mock<IParticleTrigger>();

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, System.Array.Empty<IParticleBehavior>());

            var reservation = allocator.Reserve(1);
            reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);
            reservation.GetPropertyValues<float>(StandardParmeProperties.TimeAlive.Name);
        }
        
        [Fact]
        public void Trigger_Called_If_Emitter_Is_Active()
        {
            var trigger = new Mock<IParticleTrigger>();

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, System.Array.Empty<IParticleBehavior>())
            {
                IsEmittingNewParticles = true,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Once);
        }

        [Fact]
        public void Trigger_Not_Called_If_Emitter_Is_Not_Active()
        {
            var trigger = new Mock<IParticleTrigger>();

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, System.Array.Empty<IParticleBehavior>())
            {
                IsEmittingNewParticles = false,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Never);
        }

        [Fact]
        public void Initializer_Properties_Are_Registered()
        {
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new IParticleBehavior[] {behavior.Object});

            var reservation = allocator.Reserve(1);
            reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Modifier_Properties_Are_Registered()
        {
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.ModifiedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});

            var reservation = allocator.Reserve(1);
            reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Behavior_Initialization_Method_Called_When_Trigger_Returns_A_Positive_Number_And_Initializer_Properties_Specified()
        {
            const int newParticleCount = 5;
            
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});
            
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f))
                .Returns(newParticleCount);
            
            emitter.Update(0.16f);
            
            behavior.Verify(x => 
                x.InitializeCreatedParticles(
                    emitter, 
                    It.IsAny<ParticleCollection>(),
                    It.Is<IReadOnlyList<int>>(y => y.Count == newParticleCount)),
                Times.Once);
        }
        
        [Fact]
        public void Behavior_Initialization_Method_Not_Called_When_Trigger_Returns_Zero_And_Initializer_Properties_Specified()
        {
            const int newParticleCount = 0;
            
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});
            
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f))
                .Returns(newParticleCount);
            
            emitter.Update(0.16f);
            
            behavior.Verify(x => 
                    x.InitializeCreatedParticles(
                        emitter, 
                        It.IsAny<ParticleCollection>(),
                        It.Is<IReadOnlyList<int>>(y => y.Count == newParticleCount)),
                Times.Never);
        }
        
        [Fact]
        public void Behavior_Initialization_Method_Not_Called_When_Trigger_Returns_A_Positive_Number_And_Initializer_Properties_Not_Specified()
        {
            const int newParticleCount = 5;
            
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(Array.Empty<ParticleProperty>()));
            
            behavior.Setup(x => x.ModifiedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});
            
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f))
                .Returns(newParticleCount);
            
            emitter.Update(0.16f);
            
            behavior.Verify(x => 
                    x.InitializeCreatedParticles(
                        emitter, 
                        It.IsAny<ParticleCollection>(),
                        It.Is<IReadOnlyList<int>>(y => y.Count == newParticleCount)),
                Times.Never);
        }

        [Fact]
        public void Behavior_Modifier_Method_Called_When_Modifier_Properties_Specified()
        {
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.ModifiedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});

            emitter.Update(0.16f);

            behavior.Verify(x => x.UpdateParticles(emitter, It.IsAny<ParticleCollection>(), 0.16f), Times.Once);
        }
        
        [Fact]
        public void Behavior_Modifier_Method_Not_Called_When_No_Modifier_Properties_Specified()
        {
            var trigger = new Mock<IParticleTrigger>();
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            behavior.Setup(x => x.ModifiedProperties)
                .Returns(new HashSet<ParticleProperty>(Array.Empty<ParticleProperty>()));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});

            emitter.Update(0.16f);

            behavior.Verify(x => x.UpdateParticles(emitter, It.IsAny<ParticleCollection>(), 0.16f), Times.Never);
        }

        [Fact]
        public void Initializer_Given_New_Particle_Indices_That_Were_Previously_Dead()
        {
            const int newParticleCount = 3;
            
            var trigger = new Mock<IParticleTrigger>();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object}, 10);

            var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
            values[1] = true;
            values[3] = true;
            values[5] = true;
            values[7] = true;
            values[9] = true;
            
            IReadOnlyList<int>? newParticleIndices = null;
            behavior.Setup(x =>
                    x.InitializeCreatedParticles(
                        It.IsAny<ParticleEmitter>(),
                        It.IsAny<ParticleCollection>(),
                        It.IsAny<IReadOnlyList<int>>()))
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
            
            var trigger = new Mock<IParticleTrigger>();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object}, 10);

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
            
            var trigger = new Mock<IParticleTrigger>();
            trigger.Setup(x => x.DetermineNumberOfParticlesToCreate(It.IsAny<ParticleEmitter>(), 0.16f))
                .Returns(newParticleCount);
            
            var behavior = new Mock<IParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object}, 10);

            {
                var values = emitter.Reservation.GetPropertyValues<bool>(StandardParmeProperties.IsAlive.Name);
                values[1] = true;
                values[3] = true;
                values[5] = true;
                values[7] = true;
                values[9] = true;
            }

            IReadOnlyList<int>? newParticleIndices = null;
            behavior.Setup(x =>
                    x.InitializeCreatedParticles(
                        It.IsAny<ParticleEmitter>(),
                        It.IsAny<ParticleCollection>(),
                        It.IsAny<IReadOnlyList<int>>()))
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