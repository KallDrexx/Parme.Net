using System.Collections.Generic;

namespace Parme.Net.Modifiers;

public class ApplyRotationalVelocityModifier : IParticleModifier
{
    public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
    {
        StandardParmeProperties.RotationalVelocity,
    });

    public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
    {
        StandardParmeProperties.RotationInRadians,
    });

    public IParticleModifier Clone()
    {
        return new ApplyRotationalVelocityModifier();
    }

    public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
    {
        var velocity = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.RotationalVelocity.Name);
        var rotation = particles.GetPropertyValues<float>(StandardParmeProperties.RotationInRadians.Name);

        for (var index = 0; index < particles.Count; index++)
        {
            rotation[index] += velocity[index] * secondsSinceLastUpdate;
        }
    }
}