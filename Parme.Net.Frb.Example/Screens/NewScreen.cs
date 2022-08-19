using System;
using System.Numerics;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Frb.Example.Screens
{
    public partial class NewScreen
    {
        void CustomInitialize()
        {
            ParmeEmitterManager.Instance.CreateEmitter(CreateTestEmitterConfig(), null);
        }

        void CustomActivity(bool firstTimeCalled)
        {
        }

        void CustomDestroy()
        {
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {
        }

        private static EmitterConfig CreateTestEmitterConfig()
        {
            var random = new Random();
            return new EmitterConfig
            {
                InitialCapacity = 10,
                MaxParticleLifetime = 1,
                Trigger = new TimeBasedTrigger(random)
                {
                    SecondsBetweenEmissions = 5f,
                    MinParticlesToEmit = 1,
                    MaxParticlesToEmit = 1,
                },

                Initializers =
                {
                    new ColorInitializer()
                    {
                        // orange
                        StartingRed = 255,
                        StartingGreen = 165,
                        StartingBlue = 0,
                        StartingAlpha = 1,
                    },

                    new SizeInitializer(random)
                    {
                        MinSize = new Vector2(10, 10),
                        MaxSize = new Vector2(10, 10),
                    },
                    
                    new RangedVelocityInitializer(random)
                    {
                        MinVelocity = new Vector2(10, 10),
                        MaxVelocity = new Vector2(100, 100),
                    },
                },

                Modifiers =
                {
                    new Apply2dVelocityModifier(),
                }
            };
        }
    }
}
