using Moq;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Tests
{
    public abstract class TestBase
    {
        protected const int FirstInitializedIndex = 0;
        protected const int LastInitializedIndex = 2;

        protected static Mock<ParticleTrigger> MockTrigger()
        {
            var trigger = new Mock<ParticleTrigger>();
            trigger.Setup(x => x.Clone())
                .Returns(trigger.Object);

            return trigger;
        }

        protected static Mock<IParticleInitializer> MockInitializer()
        {
            var initializer = new Mock<IParticleInitializer>();
            initializer.Setup(x => x.Clone())
                .Returns(initializer.Object);

            return initializer;
        }

        protected static Mock<IParticleModifier> MockModifier()
        {
            var modifier = new Mock<IParticleModifier>();
            modifier.Setup(x => x.Clone())
                .Returns(modifier.Object);

            return modifier;
        }

        protected static ParticleCollection RunInitializer(IParticleInitializer initializer)
        {
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

            initializer.InitializeParticles(emitter, collection, FirstInitializedIndex, LastInitializedIndex);

            return collection;
        }
    }
}