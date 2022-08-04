using System;
using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class VelocityBasedRotationModifier : IParticleModifier
    {
        public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
        {
            StandardParmeProperties.VelocityX,
            StandardParmeProperties.VelocityY,
        });

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.RotationInRadians,
        });

        public IParticleModifier Clone()
        {
            return new VelocityBasedRotationModifier();
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var velocityX = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
            var velocityY = particles.GetReadOnlyPropertyValues<float>(StandardParmeProperties.VelocityY.Name);
            var rotation = particles.GetPropertyValues<float>(StandardParmeProperties.RotationInRadians.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                if (velocityX[index] != 0 || velocityY[index] != 0)
                {
                    rotation[index] = (float)Math.Atan2(velocityY[index], velocityX[index]);
                }
            }
        }
    }
}