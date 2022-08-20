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
                    SecondsBetweenEmissions = 0.01f,
                    MinParticlesToEmit = 0,
                    MaxParticlesToEmit = 5,
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
                        MinVelocity = new Vector2(0, 100),
                        MaxVelocity = new Vector2(0, 200),
                    },
                
                    new RegionalPositionInitializer(random)
                    {
                        MinRelativePosition = new Vector2(-25, -20),
                        MaxRelativePosition = new Vector2(25, 20),
                    },
                },

                Modifiers =
                {
                    new AccelerationModifier()
                    {
                        AccelerationX = 0,
                        AccelerationY = -75,
                    },
                    
                    new ConstantSizeModifier()
                    {
                        WidthChangePerSecond = -5,
                        HeightChangePerSecond = -5,
                    },
                    
                    new EndingColorModifier()
                    {
                        EndingRed = 255,
                        EndingGreen = 165,
                        EndingBlue = 0,
                        EndingAlpha = 0,
                    },
                    
                    
                    new Apply2dVelocityModifier(),
                    // new ApplyRotationalVelocityModifier(),
                }
            };
        }
    }
}
