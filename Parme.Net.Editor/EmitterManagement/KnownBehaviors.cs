using System;
using System.Collections.Generic;
using System.Linq;
using Parme.Net.Initializers;
using Parme.Net.Modifiers;
using Parme.Net.Triggers;

namespace Parme.Net.Editor.EmitterManagement;

public class KnownBehaviors
{
    private readonly Dictionary<Type, ParticleTrigger> _triggers = new();
    private readonly Dictionary<Type, IParticleInitializer> _initializers = new();
    private readonly Dictionary<Type, IParticleModifier> _modifiers = new();

    public IReadOnlyList<Type> TriggerTypes => _triggers.Select(x => x.Key).ToArray();
    public IReadOnlyList<Type> InitializerTypes => _initializers.Select(x => x.Key).ToArray();
    public IReadOnlyList<Type> ModifierTypes => _modifiers.Select(x => x.Key).ToArray();

    public KnownBehaviors()
    {
        var random = new Random();
        Register(new DistanceBasedTrigger(random));
        Register(new OneShotTrigger(random));
        Register(new TimeBasedTrigger(random));
        
        Register(new ColorInitializer());
        Register(new RadialVelocityInitializer(random));
        Register(new RangedVelocityInitializer(random));
        Register(new RegionalPositionInitializer(random));
        Register(new RotationalVelocityInitializer(random));
        Register(new RotationInitializer(random));
        Register(new SizeInitializer(random));
        Register(new TextureInitializer(random));
        
        Register(new AccelerationModifier());
        Register(new AnimatingTextureModifier());
        Register(new Apply2dVelocityModifier());
        Register(new ApplyRotationalVelocityModifier());
        Register(new ConstantSizeModifier());
        Register(new DragModifier());
        Register(new EndingColorModifier());
        Register(new EndingSizeModifier());
        Register(new Particle2dPositionModifier());
        Register(new VelocityBasedRotationModifier());
    }

    private void Register(ParticleTrigger trigger)
    {
        _triggers.Add(trigger.GetType(), trigger);
    }

    private void Register(IParticleInitializer initializer)
    {
        _initializers.Add(initializer.GetType(), initializer);
    }

    private void Register(IParticleModifier modifier)
    {
        _modifiers.Add(modifier.GetType(), modifier);
    }

    public ParticleTrigger GetTriggerByType(Type type)
    {
        return _triggers[type].Clone();
    }

    public IParticleInitializer GetInitializerByType(Type type)
    {
        return _initializers[type].Clone();
    }

    public IParticleModifier GetModifierByType(Type type)
    {
        return _modifiers[type].Clone();
    }
}