using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class DragModifier : IParticleModifier
    {
        public float DragFactor { get; set; }

        public HashSet<ParticleProperty> PropertiesIRead { get; } = new();

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.VelocityX,
            StandardParmeProperties.VelocityY,
        });

        public IParticleModifier Clone()
        {
            return new DragModifier
            {
                DragFactor = DragFactor,
            };
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var velocityX = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityX.Name);
            var velocityY = particles.GetPropertyValues<float>(StandardParmeProperties.VelocityY.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                velocityX[index] -= DragFactor * velocityX[index] * secondsSinceLastUpdate;
                velocityY[index] -= DragFactor * velocityY[index] * secondsSinceLastUpdate;
            }
        }
    }
}