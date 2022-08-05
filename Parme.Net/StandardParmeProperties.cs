namespace Parme.Net
{
    public static class StandardParmeProperties
    {
        /// <summary>
        /// Denotes if the particle is active and visible
        /// </summary>
        public static readonly ParticleProperty IsAlive = new(typeof(bool), "IsAlive");
        
        /// <summary>
        /// Tracks how many seconds since the particle was created
        /// </summary>
        public static readonly ParticleProperty TimeAlive = new(typeof(float), "TimeAlive");
        
        /// <summary>
        /// The X coordinate in world space of the center of the particle
        /// </summary>
        public static readonly ParticleProperty PositionX = new(typeof(float), "PositionX");
        
        /// <summary>
        /// Y coordinate in world space of the center of the particle
        /// </summary>
        public static readonly ParticleProperty PositionY = new(typeof(float), "PositionY");
        
        /// <summary>
        /// How many units wide the particle was when it was initialized
        /// </summary>
        public static readonly ParticleProperty InitialWidth = new(typeof(int), "InitialWidth");
        
        /// <summary>
        /// How many units tall the particle was when it was initialized
        /// </summary>
        public static readonly ParticleProperty InitialHeight = new(typeof(int), "InitialHeight");
        
        /// <summary>
        /// How many units wide the particle currently is
        /// </summary>
        public static readonly ParticleProperty CurrentWidth = new(typeof(int), "CurrentWidth");
        
        /// <summary>
        /// How many units tall the particle currently is
        /// </summary>
        public static readonly ParticleProperty CurrentHeight = new(typeof(int), "CurrentHeight");
        
        /// <summary>
        /// How many units per second the particle is moving along the X axis
        /// </summary>
        public static readonly ParticleProperty VelocityX = new(typeof(float), "VelocityX");
        
        /// <summary>
        /// How many units per second the particle is moving along the Y axis
        /// </summary>
        public static readonly ParticleProperty VelocityY = new(typeof(float), "VelocityY");

        /// <summary>
        /// The rotation (in radians) of the particle
        /// </summary>
        public static readonly ParticleProperty RotationInRadians = new(typeof(float), "RotationInRadians");
        
        /// <summary>
        /// How fast (in radians) the particle should be rotating
        /// </summary>
        public static readonly ParticleProperty RotationalVelocity = new(typeof(float), "RotationalVelocity");

        /// <summary>
        /// The index of the texture index the particle should be rendering with
        /// </summary>
        public static readonly ParticleProperty TextureSectionIndex = new(typeof(byte), "TextureSectionIndex");
        
        public static readonly ParticleProperty InitialRed = new(typeof(byte), "InitialRed");
        public static readonly ParticleProperty InitialGreen = new(typeof(byte), "InitialGreen");
        public static readonly ParticleProperty InitialBlue = new(typeof(byte), "InitialBlue");
        public static readonly ParticleProperty InitialAlpha = new(typeof(byte), "InitialAlpha");

        public static readonly ParticleProperty CurrentRed = new(typeof(byte), "CurrentRed");
        public static readonly ParticleProperty CurrentGreen = new(typeof(byte), "CurrentGreen");
        public static readonly ParticleProperty CurrentBlue = new(typeof(byte), "CurrentBlue");
        public static readonly ParticleProperty CurrentAlpha = new(typeof(byte), "currentAlpha");
    }
}