using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class AnimatingTextureModifier : IParticleModifier
    {
        public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
        {
            StandardParmeProperties.TimeAlive,
        });

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.TextureSectionIndex,
        });

        public IParticleModifier Clone()
        {
            return new AnimatingTextureModifier();
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var timeAlive = particles.GetPropertyValues<float>(StandardParmeProperties.TimeAlive);
            var textureSections = particles.GetPropertyValues<byte>(StandardParmeProperties.TextureSectionIndex);

            for (var index = 0; index < particles.Count; index++)
            {
                textureSections[index] =
                    (byte)(timeAlive[index] / emitter.MaxParticleLifetime * emitter.TextureSections.Length);
            }
        }
    }
}