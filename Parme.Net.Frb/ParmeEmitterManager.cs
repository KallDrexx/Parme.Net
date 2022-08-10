using System;
using System.Collections.Generic;
using FlatRedBall;

namespace Parme.Net.Frb;

public class ParmeEmitterManager
{
    public const string DefaultGroupName = "Default";

    private static readonly object Padlock = new();
    private static ParmeEmitterManager _instance;
    private readonly Dictionary<string, ParmeEmitterGroup> _emitterGroups = new();
    private readonly Dictionary<ParmeFrbEmitter, ParmeEmitterGroup> _emitterToGroupMap = new();

    public static ParmeEmitterManager Instance
    {
        get
        {
            lock (Padlock)
            {
                return _instance ??= new ParmeEmitterManager();
            }
        } 
    }

    private ParmeEmitterManager()
    {
    }

    public ParmeFrbEmitter CreateEmitter(EmitterConfig config, PositionedObject parent, string groupName = null)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        
        if (string.IsNullOrWhiteSpace(groupName))
        {
            groupName = DefaultGroupName;
        }
        
        var group = GetEmitterGroup(groupName);
        var emitter = group.CreateEmitter(config, parent);
        _emitterToGroupMap.Add(emitter, group);

        return emitter;
    }

    public void DestroyEmitter(ParmeFrbEmitter emitter, bool waitForAllParticlesToDie = true)
    {
        if (emitter == null) throw new ArgumentNullException(nameof(emitter));

        if (!_emitterToGroupMap.TryGetValue(emitter, out var group))
        {
            // Nothing to do since we don't know anything about the emitter
            return;
        }
        
        group.RemoveEmitter(emitter, waitForAllParticlesToDie);
        _emitterToGroupMap.Remove(emitter);
    }

    public ParmeEmitterGroup GetEmitterGroup(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        
        if (!_emitterGroups.TryGetValue(name.Trim(), out var group))
        {
            group = new ParmeEmitterGroup(FlatRedBallServices.GraphicsDevice);
            _emitterGroups.Add(name, group);
            SpriteManager.AddDrawableBatch(group);
        }

        return group;
    }

    public void DestroyActiveEmitterGroups()
    {
        foreach (var emitterGroup in _emitterGroups.Values)
        {
            SpriteManager.RemoveDrawableBatch(emitterGroup);
        }
        
        _emitterGroups.Clear();
        _emitterToGroupMap.Clear();
    }
} 