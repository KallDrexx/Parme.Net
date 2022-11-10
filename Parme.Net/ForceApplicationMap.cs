using System.Collections.Generic;
using System.Numerics;

namespace Parme.Net;

public class ForceApplicationMap
{
    private readonly List<DefinedForce> _definedForces = new();
    
    public void Clear()
    {
        _definedForces.Clear();
    }

    public void AddForce(Vector2 minBounds, Vector2 maxBounds, Vector2 velocity)
    {
        _definedForces.Add(new DefinedForce(minBounds, maxBounds, velocity));
    }

    public Vector2 GetForceAt(float x, float y)
    {
        foreach (var force in _definedForces)
        {
            if (x >= force.MinBounds.X &&
                x <= force.MaxBounds.X &&
                y >= force.MinBounds.Y &&
                y <= force.MaxBounds.Y)
            {
                return force.Velocity;
            }
        }
        
        return Vector2.Zero;
    }

    public ForceApplicationMap Clone()
    {
        var newMap = new ForceApplicationMap();
        newMap._definedForces.AddRange(_definedForces);

        return newMap;
    }

    private struct DefinedForce
    {
        public readonly Vector2 MinBounds;
        public readonly Vector2 MaxBounds;
        public readonly Vector2 Velocity;

        public DefinedForce(Vector2 minBounds, Vector2 maxBounds, Vector2 velocity)
        {
            MinBounds = minBounds;
            MaxBounds = maxBounds;
            Velocity = velocity;
        }
    }
}