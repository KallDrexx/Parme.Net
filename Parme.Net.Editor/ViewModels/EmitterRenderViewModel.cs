using System;
using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.ViewModels;

public partial class EmitterRenderViewModel : ObservableObject
{
    [ObservableProperty] private EmitterConfig? currentEmitterConfig;

    public EmitterRenderViewModel()
    {
        currentEmitterConfig = CreateTestEmitterConfig();
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
                    MinDegreesPerSecond = 180,
                    MaxDegreesPerSecond = 180,
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
            }
        };
    }
}