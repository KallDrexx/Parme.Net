using System;

namespace Parme.Net
{
    public readonly struct ParticleProperty : IEquatable<ParticleProperty>
    {
        public Type Type { get; }
        public string Name { get; }

        public ParticleProperty(Type type, string name)
        {
            Type = type;
            Name = name;
        }
        
        public bool Equals(ParticleProperty other)
        {
            return Type == other.Type && Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is ParticleProperty other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Name);
        }
    }
}