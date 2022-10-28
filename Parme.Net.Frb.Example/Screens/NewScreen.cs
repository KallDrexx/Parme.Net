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
    public partial class NewScreen
    {
        private ParmeFrbEmitter _fireEmitter;
        
        void CustomInitialize()
        {
            _fireEmitter = ParmeEmitterManager.Instance.CreateEmitter(CreateTestEmitterConfig(), null);
        }

        void CustomActivity(bool firstTimeCalled)
        {
            var repelModifier = _fireEmitter.Emitter.GetModifierOfType<RepelFromPointModifier>();
            if (repelModifier != null)
            {
                if (GuiManager.Cursor.IsInWindow())
                {
                    var cursorPosition = GuiManager.Cursor.WorldPosition;
                    var relativePosition = new Microsoft.Xna.Framework.Vector2(
                        cursorPosition.X - _fireEmitter.XOffset,
                        cursorPosition.Y - _fireEmitter.YOffset
                    );

                    repelModifier.PointRelativeToEmitter = relativePosition;
                }
                else
                {
                    repelModifier.PointRelativeToEmitter = null;
                }
            }
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
                MaxParticleLifetime = 3,
                Trigger = new TimeBasedTrigger(random)
                {
                    SecondsBetweenEmissions = 0.01f,
                    MinParticlesToEmit = 0,
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
                        AccelerationY = -75,
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
                    new RepelFromPointModifier()
                    {
                        Radius = 100,
                        MaxMagnitude = 500,
                    },
                }
            };
        }

        private class RepelFromPointModifier : IParticleModifier
        {
            public Microsoft.Xna.Framework.Vector2? PointRelativeToEmitter { get; set; }
            public float Radius { get; set; }
            public float MaxMagnitude { get; set; }

            public HashSet<ParticleProperty> PropertiesIRead { get; } = new HashSet<ParticleProperty>();

            public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new HashSet<ParticleProperty>(new[]
            {
                StandardParmeProperties.PositionX,
                StandardParmeProperties.PositionY,
            });

            public IParticleModifier Clone()
            {
                return new RepelFromPointModifier()
                {
                    PointRelativeToEmitter = PointRelativeToEmitter,
                    Radius = Radius,
                    MaxMagnitude = MaxMagnitude,
                };
            }

            public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
            {
                if (PointRelativeToEmitter == null || Radius <= 0f)
                {
                    return;
                }
                
                var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX);
                var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY);

                for (var index = 0; index < particles.Count; index++)
                {
                    var particlePosition = new Microsoft.Xna.Framework.Vector2(positionX[index], positionY[index]);
                    var centerToPosition = particlePosition - PointRelativeToEmitter.Value;
                    var distance = centerToPosition.Length();

                    var magnitude = (1 - Math.Min(distance / Radius, 1)) * MaxMagnitude;
                    var velocityDirection = centerToPosition.NormalizedOrRight();
                    var velocity = velocityDirection * magnitude;

                    positionX[index] += velocity.X * secondsSinceLastUpdate;
                    positionY[index] += velocity.Y * secondsSinceLastUpdate;
                }
            }
        }
    }
}
