using System;
using System.Collections.Generic;
using Moq;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;
using Xunit;

namespace Parme.Net.Tests
{
    public class EmitterTests
    {
        [Fact]
        public void Trigger_Called_If_Emitter_Is_Active()
        {
            var trigger = new Mock<ParticleTrigger>();

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, System.Array.Empty<ParticleBehavior>())
            {
                IsEmittingNewParticles = true,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Once);
        }

        [Fact]
        public void Trigger_Not_Called_If_Emitter_Is_Not_Active()
        {
            var trigger = new Mock<ParticleTrigger>();

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, System.Array.Empty<ParticleBehavior>())
            {
                IsEmittingNewParticles = false,
            };
            
            emitter.Update(0.16f);

            trigger.Verify(x => x.DetermineNumberOfParticlesToCreate(emitter, 0.16f), Times.Never);
        }

        [Fact]
        public void Initializer_Properties_Are_Registered()
        {
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));

            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new ParticleBehavior[] {behavior.Object});

            var reservation = allocator.Reserve(1);
            reservation.GetPropertyValues<bool>("Test1");
        }

        [Fact]
        public void Modifier_Properties_Are_Registered()
        {
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
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
            
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
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
            
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
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
            
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
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
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
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
            var trigger = new Mock<ParticleTrigger>();
            var behavior = new Mock<ParticleBehavior>();
            behavior.Setup(x => x.InitializedProperties)
                .Returns(new HashSet<ParticleProperty>(new[] {new ParticleProperty(typeof(bool), "Test1")}));
            
            behavior.Setup(x => x.ModifiedProperties)
                .Returns(new HashSet<ParticleProperty>(Array.Empty<ParticleProperty>()));
            
            var allocator = new ParticleAllocator(10);
            var emitter = new ParticleEmitter(allocator, trigger.Object, new[] {behavior.Object});

            emitter.Update(0.16f);

            behavior.Verify(x => x.UpdateParticles(emitter, It.IsAny<ParticleCollection>(), 0.16f), Times.Never);
        }
    }
}