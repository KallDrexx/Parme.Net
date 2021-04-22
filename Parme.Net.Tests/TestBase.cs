using Moq;
using Parme.Net.Behaviors;
using Parme.Net.Triggers;

namespace Parme.Net.Tests
{
    public abstract class TestBase
    {
        protected static Mock<ParticleTrigger> MockTrigger()
        {
            var trigger = new Mock<ParticleTrigger>();
            trigger.Setup(x => x.Clone())
                .Returns(trigger.Object);

            return trigger;
        }

        protected static Mock<ParticleBehavior> MockBehavior()
        {
            var behavior = new Mock<ParticleBehavior>();
            behavior.Setup(x => x.Clone())
                .Returns(behavior.Object);

            return behavior;
        }
    }
}