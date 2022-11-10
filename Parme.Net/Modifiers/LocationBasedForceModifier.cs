using System.Collections.Generic;

namespace Parme.Net.Modifiers;

public class LocationBasedForceModifier : IParticleModifier
{
    public ForceApplicationMap ForceMap { get; private init; } = new();

    public HashSet<ParticleProperty> PropertiesIRead { get; } = new();

    public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
    {
        StandardParmeProperties.PositionX,
        StandardParmeProperties.PositionY,
    });

    public IParticleModifier Clone()
    {
        return new LocationBasedForceModifier
        {
            ForceMap = ForceMap.Clone()
        };
    }

    public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
    {
        var positionX = particles.GetPropertyValues<float>(StandardParmeProperties.PositionX);
        var positionY = particles.GetPropertyValues<float>(StandardParmeProperties.PositionY);

        for (var index = 0; index < particles.Count; index++)
        {
            var velocity = ForceMap.GetForceAt(positionX[index], positionY[index]);
            positionX[index] += velocity.X * secondsSinceLastUpdate;
            positionY[index] += velocity.Y * secondsSinceLastUpdate;
        }
    }
}