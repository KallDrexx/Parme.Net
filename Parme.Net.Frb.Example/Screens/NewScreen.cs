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
                MaxParticleLifetime = 5,
                Trigger = new TimeBasedTrigger(random)
                {
                    SecondsBetweenEmissions = 0.25f,
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
                        StartingAlpha = 255,
                    },

                    new SizeInitializer(random)
                    {
                        MinSize = new Vector2(10, 10),
                        MaxSize = new Vector2(10, 10),
                    },
                    
                    new RangedVelocityInitializer(random)
                    {
                        MinVelocity = new Vector2(50, 50),
                        MaxVelocity = new Vector2(50, 50),
                    },
                
                    new RegionalPositionInitializer(random)
                    {
                        // MinRelativePosition = new Vector2(-25, 25),
                        // MaxRelativePosition = new Vector2(-20, 20),
                    },
                },

                Modifiers =
                {
                    new EndingColorModifier()
                    {
                        EndingRed = 255,
                        EndingGreen = 255,
                        EndingBlue = 255,
                        EndingAlpha = 255,
                    },
                    
                    new Apply2dVelocityModifier(),
                    new ApplyRotationalVelocityModifier(),
                }
            };
        }
    }
}
