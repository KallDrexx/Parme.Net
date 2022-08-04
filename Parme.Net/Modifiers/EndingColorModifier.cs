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
            var initialRed = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialRed.Name);
            var initialGreen = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialGreen.Name);
            var initialBlue = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialBlue.Name);
            var initialAlpha = particles.GetReadOnlyPropertyValues<byte>(StandardParmeProperties.InitialAlpha.Name);
            var currentRed = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentRed.Name);
            var currentGreen = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentGreen.Name);
            var currentBlue = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentBlue.Name);
            var currentAlpha = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentAlpha.Name);

            for (var index = 0; index < particles.Count; index++)
            {
                var red = (initialRed[index] - EndingRed) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var green = (initialGreen[index] - EndingGreen) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var blue = (initialBlue[index] - EndingBlue) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;
                var alpha = (initialAlpha[index] - EndingAlpha) / emitter.MaxParticleLifetime * secondsSinceLastUpdate;

                currentRed[index] -= (byte)red;
                currentGreen[index] -= (byte)green;
                currentBlue[index] -= (byte)blue;
                currentAlpha[index] -= (byte)alpha;
            }
        }
    }
}