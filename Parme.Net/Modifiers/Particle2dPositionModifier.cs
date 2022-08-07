using System.Collections.Generic;

namespace Parme.Net.Modifiers;

/// <summary>
/// Updates a particle's 2d position based on it's velocity
/// </summary>
public class Particle2dPositionModifier : IParticleModifier
{
    public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
    {
        StandardParmeProperties.VelocityX,
        StandardParmeProperties.VelocityY,
    });

    public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
    {
        StandardParmeProperties.PositionX,
        StandardParmeProperties.PositionY,
    });

    public IParticleModifier Clone()
    {
        return new Particle2dPositionModifier();
    }

    public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
    {
        var velocityX = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
        var velocityY = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityY.Name);
        var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX.Name);
        var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY.Name);

        for (var index = 0; index < particles.Count; index++)
        {
            positionX[index] += velocityX[index] * secondsSinceLastUpdate;
            positionY[index] += velocityY[index] * secondsSinceLastUpdate;
        }
    }
}