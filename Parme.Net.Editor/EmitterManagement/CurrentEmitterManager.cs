using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CommunityToolkit.Mvvm.Messaging;
using Parme.Net.Editor.Messages;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.EmitterManagement;

public record TaggedInitializer(Guid Id, IParticleInitializer Initializer, bool IsEnabled = true);

public record TaggedModifier(Guid Id, IParticleModifier Modifier, bool IsEnabled = true);

public record EmitterSettings(float MaxParticleLifetime);

public class CurrentEmitterManager : 
    IRecipient<CurrentEmitterConfigRequestMessage>,
    IRecipient<CurrentModifiersRequestMessage>,
    IRecipient<CurrentInitializersRequestMessage>,
    IRecipient<GetCurrentTriggerRequest>,
    IRecipient<CurrentEmitterSettingsRequest>,
    IRecipient<UpdatedEmitterSettings>,
    IRecipient<GetModifierDetailsRequest>,
    IRecipient<GetInitializerDetailsRequest>
{
    private EmitterSettings _settings;
    private ParticleTrigger? _trigger;
    private readonly List<TaggedInitializer> _initializers = new();
    private readonly List<TaggedModifier> _modifiers = new();

    public CurrentEmitterManager()
    {
        var config = CreateTestEmitterConfig();
        _settings = new EmitterSettings(config.MaxParticleLifetime);
        _trigger = config.Trigger?.Clone();
        _initializers.AddRange(config.Initializers
            .Select(x => new TaggedInitializer(Guid.NewGuid(), x.Clone())));
        
        _modifiers.AddRange(config.Modifiers
            .Select(x => new TaggedModifier(Guid.NewGuid(), x.Clone())));
        
        EmitterConfigUpdated();
        
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(CurrentEmitterConfigRequestMessage message)
    {
        var emitterConfig = FormEmitterConfig();
        message.Reply(emitterConfig);
    }

    public void Receive(CurrentModifiersRequestMessage message)
    {
        message.Reply(_modifiers.ToArray());
    }

    public void Receive(CurrentInitializersRequestMessage message)
    {
        message.Reply(_initializers.ToArray());
    }

    public void Receive(GetCurrentTriggerRequest message)
    {
        message.Reply(_trigger?.Clone());
    }

    public void Receive(CurrentEmitterSettingsRequest message)
    {
        message.Reply(_settings);
    }

    public void Receive(UpdatedEmitterSettings message)
    {
        _settings = message.Value;
        EmitterConfigUpdated();
    }

    public void Receive(GetModifierDetailsRequest message)
    {
        var modifier = _modifiers.SingleOrDefault(x => x.Id == message.ItemId);
        message.Reply(modifier);
    }

    public void Receive(GetInitializerDetailsRequest message)
    {
        var initializer = _initializers.SingleOrDefault(x => x.Id == message.ItemId);
        message.Reply(initializer);
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
    
    private void EmitterConfigUpdated()
    {
        var emitterConfig = FormEmitterConfig();

        WeakReferenceMessenger.Default
            .Send(new EmitterConfigChangedMessage(emitterConfig));

        WeakReferenceMessenger.Default
            .Send(new ModifiersChangedMessage(_modifiers.ToArray()));

        WeakReferenceMessenger.Default
            .Send(new InitializersChangedMessage(_initializers.ToArray()));
    }

    private EmitterConfig FormEmitterConfig()
    {
        var emitterConfig = new EmitterConfig
        {
            InitialCapacity = 50,
            MaxParticleLifetime = _settings.MaxParticleLifetime,
            Trigger = _trigger?.Clone()
        };

        emitterConfig.Initializers.AddRange(_initializers
            .Where(x => x.IsEnabled)
            .Select(x => x.Initializer.Clone())
            .ToArray());

        emitterConfig.Modifiers.AddRange(_modifiers
            .Where(x => x.IsEnabled)
            .Select(x => x.Modifier.Clone())
            .ToArray());
        return emitterConfig;
    }
}