using System.Collections.Generic;

namespace Parme.Net.Initializers
{
    public class ColorInitializer : IParticleInitializer
    {
        public byte StartingRed { get; set; }
        public byte StartingGreen { get; set; }
        public byte StartingBlue { get; set; }
        public float StartingAlpha { get; set; }
        
        public HashSet<ParticleProperty> PropertiesISet { get; } = new(new[]
        {
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
            var red = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentRed.Name);
            var green = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentGreen.Name);
            var blue = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentBlue.Name);
            var alpha = particles.GetPropertyValues<byte>(StandardParmeProperties.CurrentAlpha.Name);

            foreach (var index in newParticleIndices)
            {
                red[index] = StartingRed;
                green[index] = StartingGreen;
                blue[index] = StartingBlue;
                alpha[index] = (byte)(StartingAlpha * 255);
            }
        }
    }
}