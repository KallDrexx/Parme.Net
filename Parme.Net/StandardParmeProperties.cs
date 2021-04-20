namespace Parme.Net
{
    public static class StandardParmeProperties
    {
        public static readonly ParticleProperty IsAlive = new ParticleProperty(typeof(bool), "IsAlive");
        public static readonly ParticleProperty TimeAlive = new ParticleProperty(typeof(float), "TimeAlive");
        public static readonly ParticleProperty PositionX = new ParticleProperty(typeof(float), "PositionX");
        public static readonly ParticleProperty PositionY = new ParticleProperty(typeof(float), "PositionY");
    }
}