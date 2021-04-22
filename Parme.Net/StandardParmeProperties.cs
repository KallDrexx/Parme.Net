namespace Parme.Net
{
    public static class StandardParmeProperties
    {
        /// <summary>
        /// Denotes if the particle is active and visible
        /// </summary>
        public static readonly ParticleProperty IsAlive = new ParticleProperty(typeof(bool), "IsAlive");
        
        /// <summary>
        /// Tracks how many seconds since the particle was created
        /// </summary>
        public static readonly ParticleProperty TimeAlive = new ParticleProperty(typeof(float), "TimeAlive");
        
        /// <summary>
        /// The X coordinate in world space of the center of the particle
        /// </summary>
        public static readonly ParticleProperty PositionX = new ParticleProperty(typeof(float), "PositionX");
        
        /// <summary>
        /// Y coordinate in world space of the center of the particle
        /// </summary>
        public static readonly ParticleProperty PositionY = new ParticleProperty(typeof(float), "PositionY");
        
        /// <summary>
        /// How many units wide the particle was when it was initialized
        /// </summary>
        public static readonly ParticleProperty InitialWidth = new ParticleProperty(typeof(float), "InitialWidth");
        
        /// <summary>
        /// How many units tall the particle was when it was initialized
        /// </summary>
        public static readonly ParticleProperty InitialHeight = new ParticleProperty(typeof(float), "InitialHeight");
        
        /// <summary>
        /// How many units wide the particle currently is
        /// </summary>
        public static readonly ParticleProperty CurrentWidth = new ParticleProperty(typeof(float), "CurrentWidth");
        
        /// <summary>
        /// How many units tall the particle currently is
        /// </summary>
        public static readonly ParticleProperty CurrentHeight = new ParticleProperty(typeof(float), "CurrentHeight");
        
        /// <summary>
        /// How many units per second the particle is moving along the X axis
        /// </summary>
        public static readonly ParticleProperty VelocityX = new ParticleProperty(typeof(float), "VelocityX");
        
        /// <summary>
        /// How many units per second the particle is moving along the Y axis
        /// </summary>
        public static readonly ParticleProperty VelocityY = new ParticleProperty(typeof(float), "VelocityY");
    }
}