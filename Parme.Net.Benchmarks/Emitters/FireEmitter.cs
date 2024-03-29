﻿using System.Numerics;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Benchmarks.Emitters;

public static class FireEmitter 
{
    public static ParticleEmitter Create(Random random, ParticleAllocator allocator, int initialCapacity)
    {
        var config = new EmitterConfig
        {
            InitialCapacity = initialCapacity,
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
                    StartingAlpha = 1,
                },

                new RangedVelocityInitializer(random)
                {
                    MinVelocity = new Vector2(0, 0),
                    MaxVelocity = new Vector2(100, 200),
                },

                new RegionalPositionInitializer(random)
                {
                    MinRelativePosition = new Vector2(-25, 25),
                    MaxRelativePosition = new Vector2(-20, 20)
                },

                new SizeInitializer(random)
                {
                    MinSize = new Vector2(10, 10),
                    MaxSize = new Vector2(10, 10),
                },
            },

            Modifiers =
            {
                new AccelerationModifier()
                {
                    AccelerationX = -75,
                    AccelerationY = 0,
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
                }
            }
        };

        return new ParticleEmitter(allocator, config);
    }
}