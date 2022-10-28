using System.Collections.Generic;

namespace Parme.Net.Modifiers;

public class Apply2dVelocityModifier : IParticleModifier
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
        return new Apply2dVelocityModifier();
    }

    public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
    {
        var velocityX = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityX);
        var velocityY = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityY);
        var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX);
        var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY);

        for (var index = 0; index < particles.Count; index++)
        {
            positionX[index] += velocityX[index] * secondsSinceLastUpdate;
            positionY[index] += velocityY[index] * secondsSinceLastUpdate;
        }
    }
}