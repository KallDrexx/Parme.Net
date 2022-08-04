using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class EndingSizeModifier : IParticleModifier
    {
        public int EndingWidth { get; set; }
        public int EndingHeight { get; set; }

        public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
        {
            StandardParmeProperties.InitialHeight,
            StandardParmeProperties.InitialWidth,
        });

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.CurrentHeight,
            StandardParmeProperties.CurrentWidth,
        });

        public IParticleModifier Clone()
        {
            return new EndingSizeModifier
            {
                EndingWidth = EndingWidth,
                EndingHeight = EndingHeight,
            };
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var initialHeight = particles.GetReadOnlyPropertyValues<int>(StandardParmeProperties.InitialHeight.Name);
            var initialWidth = particles.GetReadOnlyPropertyValues<int>(StandardParmeProperties.InitialWidth.Name);
            var currentHeight = particles.GetPropertyValues<int>(StandardParmeProperties.CurrentHeight.Name);
            var currentWidth = particles.GetPropertyValues<int>(StandardParmeProperties.CurrentWidth.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                var width = (initialWidth[index] - EndingWidth) / emitter.MaxParticleLifetime *
                            secondsSinceLastUpdate;

                var height = (initialHeight[index] - EndingHeight) / emitter.MaxParticleLifetime *
                             secondsSinceLastUpdate;

                currentWidth[index] -= (int)width;
                currentHeight[index] -= (int)height;
            }
        }
    }
}