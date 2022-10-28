using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    public class ColorInitializer : IParticleInitializer
    {
        public byte StartingRed { get; set; }
        public byte StartingGreen { get; set; }
        public byte StartingBlue { get; set; }
        public byte StartingAlpha { get; set; }
        
        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
            StandardParmeProperties.InitialRed,
            StandardParmeProperties.InitialGreen,
            StandardParmeProperties.InitialBlue,
            StandardParmeProperties.InitialAlpha,
            StandardParmeProperties.CurrentRed,
            StandardParmeProperties.CurrentGreen,
            StandardParmeProperties.CurrentBlue,
            StandardParmeProperties.CurrentAlpha,
        });

        public IParticleInitializer Clone()
        {
            return new ColorInitializer()
            {
                StartingRed = StartingRed,
                StartingGreen = StartingGreen,
                StartingBlue = StartingBlue,
                StartingAlpha = StartingAlpha,
            };
        }

        public void InitializeParticles(ParticleEmitter emitter, ParticleCollection particles, IReadOnlyList<int> newParticleIndices)
        {
            var initialRed = particles.GetPropertyValues<byte>(StandardParmeProperties.InitialRed);
            var initialGreen = particles.GetPropertyValues<byte>(StandardParmeProperties.InitialGreen);
            var initialBlue = particles.GetPropertyValues<byte>(StandardParmeProperties.InitialBlue);
            var initialAlpha = particles.GetPropertyValues<byte>(StandardParmeProperties.InitialAlpha);
            var currentRed = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentRed);
            var currentGreen = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentGreen);
            var currentBlue = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentBlue);
            var currentAlpha = particles.GetPropertyValues<float>(StandardParmeProperties.CurrentAlpha);

            foreach (var index in newParticleIndices)
            {
                initialRed[index] = StartingRed;
                initialGreen[index] = StartingGreen;
                initialBlue[index] = StartingBlue;
                initialAlpha[index] = StartingAlpha;
                currentRed[index] = StartingRed;
                currentGreen[index] = StartingGreen;
                currentBlue[index] = StartingBlue;
                currentAlpha[index] = StartingAlpha;
            }
        }
    }
}