using System;
using System.Collections.Generic;
using FlatRedBall.Gui;
using Microsoft.Xna.Framework;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;
using Vector2 = System.Numerics.Vector2;

namespace Parme.Net.Frb.Example.Screens
{
    public partial class ForceCollisionDemoScreen
    {
        private ParmeFrbEmitter _fireEmitter;
        
        void CustomInitialize()
        {
            _fireEmitter = ParmeEmitterManager.Instance.CreateEmitter(CreateTestEmitterConfig(), null);
            var modifier = _fireEmitter.Emitter.GetBehavior<LocationBasedForceModifier>()
                           ?? throw new InvalidOperationException("No location based force modifier found");
            
            modifier.ForceMap.AddForce(
                new Vector2(
                    FanRightInstance.X - FanRightInstance.ForceAreaWidth / 2, 
                    FanRightInstance.Y - FanRightInstance.ForceAreaHeight / 2),
                new Vector2(
                    FanRightInstance.X + FanRightInstance.ForceAreaWidth / 2,
                    FanRightInstance.Y + FanRightInstance.ForceAreaHeight / 2),
                new Vector2(FanRightInstance.ForceVelocityX, FanRightInstance.ForceVelocityY));
            
            modifier.ForceMap.AddForce(
                new Vector2(
                    FanLeftInstance.X - FanLeftInstance.ForceAreaWidth / 2, 
                    FanLeftInstance.Y - FanLeftInstance.ForceAreaHeight / 2),
                new Vector2(
                    FanLeftInstance.X + FanLeftInstance.ForceAreaWidth / 2,
                    FanLeftInstance.Y + FanLeftInstance.ForceAreaHeight / 2),
                new Vector2(FanLeftInstance.ForceVelocityX, FanLeftInstance.ForceVelocityY));
        }

        void CustomActivity(bool firstTimeCalled)
        {
            if (GuiManager.Cursor.IsInWindow())
            {
                var cursorPosition = GuiManager.Cursor.WorldPosition;
                _fireEmitter.XOffset = cursorPosition.X;
                _fireEmitter.YOffset = cursorPosition.Y;
            }
        }

        void CustomDestroy()
        {
            ParmeEmitterManager.Instance.DestroyActiveEmitterGroups();
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
                MaxParticleLifetime = 10,
                Trigger = new TimeBasedTrigger(random)
                {
                    SecondsBetweenEmissions = 0.01f,
                    MinParticlesToEmit = 1,
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
                    
                    new RotationalVelocityInitializer(random)
                    {
                        MinDegreesPerSecond = 2,
                        MaxDegreesPerSecond = 2,
                    }
                },

                Modifiers =
                {
                    new AccelerationModifier()
                    {
                        AccelerationX = 0,
                        AccelerationY = 0,
                    },
                    
                    new EndingSizeModifier()
                    {
                        EndingWidth = 0,
                        EndingHeight = 0,
                    },
                    
                    new EndingColorModifier()
                    {
                        EndingRed = 255,
                        EndingGreen = 165,
                        EndingBlue = 0,
                        EndingAlpha = 0,
                    },
                    
                    new Apply2dVelocityModifier(),
                    new ApplyRotationalVelocityModifier(),
                    new LocationBasedForceModifier(),
                }
            };
        }

    }
}
