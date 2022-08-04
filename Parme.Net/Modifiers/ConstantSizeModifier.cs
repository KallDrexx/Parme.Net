using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class ConstantSizeModifier : IParticleModifier
    {
        public float WidthChangePerSecond { get; set; }
        public float HeightChangePerSecond { get; set; }

        public HashSet<ParticleProperty> PropertiesIRead { get; } = new();

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.CurrentWidth,
            StandardParmeProperties.CurrentHeight,
        });

        public IParticleModifier Clone()
        {
            return new ConstantSizeModifier
            {
                WidthChangePerSecond = WidthChangePerSecond,
                HeightChangePerSecond = HeightChangePerSecond,
            };
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var width = particles.GetPropertyValues<int>(StandardParmeProperties.CurrentWidth.Name);
            var height = particles.GetPropertyValues<int>(StandardParmeProperties.CurrentHeight.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                width[index] += (int)(secondsSinceLastUpdate * WidthChangePerSecond);
                height[index] += (int)(secondsSinceLastUpdate * HeightChangePerSecond);
            }
        }
    }
}