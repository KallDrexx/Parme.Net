using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class AccelerationModifier : IParticleModifier
    {
        public float AccelerationX { get; set; }
        public float AccelerationY { get; set; }

        public HashSet<ParticleProperty> PropertiesIRead { get; } = new();

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.VelocityX,
            StandardParmeProperties.VelocityY,
        });

        public IParticleModifier Clone()
        {
            return new AccelerationModifier
            {
                AccelerationX = AccelerationX,
                AccelerationY = AccelerationY,
            };
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var velocityX = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
            var velocityY = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityY.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                velocityX[index] += secondsSinceLastUpdate * AccelerationX;
                velocityY[index] += secondsSinceLastUpdate * AccelerationY;
            }
        }
    }
}