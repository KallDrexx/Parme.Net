using System.Collections.Generic;

namespace Parme.Net.Modifiers
{
    public class EndingColorModifier : IParticleModifier
    {
        public byte EndingRed { get; set; }
        public byte EndingGreen { get; set; }
        public byte EndingBlue { get; set; }
        public byte EndingAlpha { get; set; }

        public HashSet<ParticleProperty> PropertiesIRead { get; } = new(new[]
        {
            StandardParmeProperties.InitialRed,
            StandardParmeProperties.InitialGreen,
            StandardParmeProperties.InitialBlue,
            StandardParmeProperties.InitialAlpha,
        });

        public HashSet<ParticleProperty> PropertiesIUpdate { get; } = new(new[]
        {
            StandardParmeProperties.CurrentRed,
            StandardParmeProperties.CurrentGreen,
            StandardParmeProperties.CurrentBlue,
            StandardParmeProperties.CurrentAlpha,
        });

        public IParticleModifier Clone()
        {
            return new EndingColorModifier
            {
                EndingRed = EndingRed,
                EndingGreen = EndingGreen,
                EndingBlue = EndingBlue,
                EndingAlpha = EndingAlpha,
            };
        }

        public void Update(ParticleEmitter emitter, ParticleCollection particles, float secondsSinceLastUpdate)
        {
            var initialRed = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialRed);
            var initialGreen = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialGreen);
            var initialBlue = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialBlue);
            var initialAlpha = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialAlpha);
            var currentRed = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentRed);
            var currentGreen = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentGreen);
            var currentBlue = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentBlue);
            var currentAlpha = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentAlpha);

            for (var index = 0; index < particles.Count; index++)
            {
                var red = (initialRed[index] - EndingRed) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var green = (initialGreen[index] - EndingGreen) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var blue = (initialBlue[index] - EndingBlue) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var alpha = (initialAlpha[index] - EndingAlpha) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;

                currentRed[index] -= red;
                currentGreen[index] -= green;
                currentBlue[index] -= blue;
                currentAlpha[index] -= alpha;
            }
        }
    }
}