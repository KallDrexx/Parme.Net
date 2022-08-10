using System;
using System.Collections.Generic;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Parme.Net.Frb;

public class ParmeEmitterGroup : IDrawableBatch
{
    private readonly ParticleCamera _particleCamera = new();
    private readonly ParticleAllocator _particleAllocator = new(100);
    private readonly List<ParmeFrbEmitter> _frbEmitters = new();
    private readonly List<ParmeFrbEmitter> _frbEmittersWaitingToBeKilled = new();
    private readonly SpriteBatch _spriteBatch;
    private readonly BasicEffect _basicEffect;
    
    public bool UpdateEveryFrame => true;
    
    /// <summary>
    /// Position of the emitter group on the X axis.  This has no effect
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Position of the emitter group on the Y axis.  This has no effect
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Position of the emitter group on the Z axis.  This is used for render sort order for all particles within
    /// this group in relation to other sprites, IDBs, and sets of particles.
    /// </summary>
    public float Z { get; set; }

    public ParmeEmitterGroup(GraphicsDevice graphicsDevice)
    {
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _basicEffect = new BasicEffect(graphicsDevice);
    }

    public void Draw(Camera camera)
    {
        _particleCamera.Origin = new System.Numerics.Vector2(camera.Position.X, camera.Position.Y);
        _particleCamera.PixelWidth = camera.DestinationRectangle.Width;
        _particleCamera.PixelHeight = camera.DestinationRectangle.Height;
        _particleCamera.HorizontalZoomFactor = camera.DestinationRectangle.Width / camera.OrthogonalWidth;
        _particleCamera.VerticalZoomFactor = camera.DestinationRectangle.Height / camera.OrthogonalHeight;

        ResetBasicEffect();
        _spriteBatch.Begin(blendState: BlendState.NonPremultiplied,
            effect: _basicEffect,
            samplerState: FlatRedBallServices.GraphicsDevice.SamplerStates[0]);
        
        foreach (var emitter in _frbEmitters)
        {
            emitter.Render(_particleCamera, _spriteBatch);
        }

        foreach (var emitter in _frbEmittersWaitingToBeKilled)
        {
            emitter.Render(_particleCamera, _spriteBatch);
        }
        
        _spriteBatch.End();
    }

    public void Update()
    {
        var isPaused = ScreenManager.CurrentScreen?.IsPaused == true;
            
        foreach (var frbEmitter in _frbEmitters)
        {
            if (isPaused && frbEmitter.StopsOnScreenPause)
            {
                continue;
            }
                
            frbEmitter.UpdatePosition();
            frbEmitter.Emitter.Update(TimeManager.SecondDifference);
        }

        for (var x = _frbEmittersWaitingToBeKilled.Count - 1; x >= 0; x--)
        {
            var emitter = _frbEmittersWaitingToBeKilled[x];
            if (emitter.IsEmitting)
            {
                // We want to make sure is emitting is off as of now, this guarantees one last emission round
                // (via the update call above) before we stop emitting.  This is required for one shot emitters
                // that will only emit when the object it's attached to is destroyed.  This also helps make sure
                // someone doesn't accidentally turn an emitter back on while we are waiting for it to be destroyed,
                // which will cause it to never go away.
                emitter.IsEmitting = false;
            }
                
            if (!emitter.Emitter.HasAnyLiveParticles())
            {
                DestroyEmitter(emitter);
            }
        } 
    }

    public void Destroy()
    {
        foreach (var frbEmitter in _frbEmitters)
        {
            frbEmitter.Emitter.Dispose();
        }
        
        foreach (var frbEmitter in _frbEmittersWaitingToBeKilled)
        {
            frbEmitter.Emitter.Dispose();
        }
            
        _frbEmitters.Clear();
        _frbEmittersWaitingToBeKilled.Clear();
    }
    
    public ParmeFrbEmitter CreateEmitter(EmitterConfig config, PositionedObject parent = null)
    {
        var emitter = new ParmeFrbEmitter(_particleAllocator, config)
        {
            Parent = parent
        };
            
        _frbEmitters.Add(emitter);

        return emitter;
    }
    
    public void RemoveEmitter(ParmeFrbEmitter emitter, bool waitTillAllParticlesDie)
    {
        if (emitter == null) throw new ArgumentNullException(nameof(emitter));

        if (waitTillAllParticlesDie)
        {
            _frbEmittersWaitingToBeKilled.Add(emitter);
        }
        else
        {
            DestroyEmitter(emitter);
        }
    }

    private void ResetBasicEffect()
    {
        var totalHorizontalZoomFactor = _particleCamera.HorizontalZoomFactor;
        var totalVerticalZoomFactor = _particleCamera.VerticalZoomFactor;
        
        _basicEffect.TextureEnabled = true;
        _basicEffect.LightingEnabled = false;
        _basicEffect.FogEnabled = false;
        _basicEffect.VertexColorEnabled = true;
        _basicEffect.World = Matrix.Identity;
        _basicEffect.Projection =
            Matrix.CreateOrthographic(_particleCamera.PixelWidth, -_particleCamera.PixelHeight, -1, 1);
        
        _basicEffect.View =
            Matrix.CreateTranslation(
                -_particleCamera.Origin.X, 
                _particleCamera.Origin.Y,
                0) *
            Matrix.CreateScale(totalHorizontalZoomFactor, totalVerticalZoomFactor, 1);
    }
    
    private void DestroyEmitter(ParmeFrbEmitter emitter)
    {
        _frbEmitters.Remove(emitter);
        _frbEmittersWaitingToBeKilled.Remove(emitter);

        emitter.Emitter.Dispose();
    }
}