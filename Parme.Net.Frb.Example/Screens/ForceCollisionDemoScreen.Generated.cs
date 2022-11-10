#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
#define SUPPORTS_GLUEVIEW_2
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using FlatRedBall;
using System;
using System.Collections.Generic;
using System.Text;
namespace Parme.Net.Frb.Example.Screens
{
    public partial class ForceCollisionDemoScreen : FlatRedBall.Screens.Screen
    {
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        
        private Parme.Net.Frb.Example.Entities.FanRight FanRightInstance;
        private Parme.Net.Frb.Example.Entities.FanRight FanLeftInstance;
        public ForceCollisionDemoScreen () 
        	: this ("ForceCollisionDemoScreen")
        {
        }
        public ForceCollisionDemoScreen (string contentManagerName) 
        	: base (contentManagerName)
        {
        }
        public override void Initialize (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            FanRightInstance = new Parme.Net.Frb.Example.Entities.FanRight(ContentManagerName, false);
            FanRightInstance.Name = "FanRightInstance";
            FanRightInstance.CreationSource = "Glue";
            FanLeftInstance = new Parme.Net.Frb.Example.Entities.FanRight(ContentManagerName, false);
            FanLeftInstance.Name = "FanLeftInstance";
            FanLeftInstance.CreationSource = "Glue";
            
            
            PostInitialize();
            base.Initialize(addToManagers);
            if (addToManagers)
            {
                AddToManagers();
            }
        }
        public override void AddToManagers () 
        {
            FanRightInstance.AddToManagers(mLayer);
            FanLeftInstance.AddToManagers(mLayer);
            base.AddToManagers();
            AddToManagersBottomUp();
            BeforeCustomInitialize?.Invoke();
            CustomInitialize();
        }
        public override void Activity (bool firstTimeCalled) 
        {
            if (!IsPaused)
            {
                
                FanRightInstance.Activity();
                FanLeftInstance.Activity();
            }
            else
            {
            }
            base.Activity(firstTimeCalled);
            if (!IsActivityFinished)
            {
                CustomActivity(firstTimeCalled);
            }
        }
        public override void ActivityEditMode () 
        {
            if (FlatRedBall.Screens.ScreenManager.IsInEditMode)
            {
                foreach (var item in FlatRedBall.SpriteManager.ManagedPositionedObjects)
                {
                    if (item is FlatRedBall.Entities.IEntity entity)
                    {
                        entity.ActivityEditMode();
                    }
                }
                CustomActivityEditMode();
                base.ActivityEditMode();
            }
        }
        public override void Destroy () 
        {
            base.Destroy();
            
            if (FanRightInstance != null)
            {
                FanRightInstance.Destroy();
                FanRightInstance.Detach();
            }
            if (FanLeftInstance != null)
            {
                FanLeftInstance.Destroy();
                FanLeftInstance.Detach();
            }
            FlatRedBall.Math.Collision.CollisionManager.Self.Relationships.Clear();
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.X = -50f;
            }
            else
            {
                FanRightInstance.RelativeX = -50f;
            }
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.Y = -100f;
            }
            else
            {
                FanRightInstance.RelativeY = -100f;
            }
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.Z = 0f;
            }
            else
            {
                FanRightInstance.RelativeZ = 0f;
            }
            FanRightInstance.ForceVelocityX = 200f;
            FanRightInstance.ForceVelocityY = 0f;
            FanLeftInstance.TextInstanceDisplayText = "<- Fan";
            if (FanLeftInstance.Parent == null)
            {
                FanLeftInstance.X = 50f;
            }
            else
            {
                FanLeftInstance.RelativeX = 50f;
            }
            if (FanLeftInstance.Parent == null)
            {
                FanLeftInstance.Y = 100f;
            }
            else
            {
                FanLeftInstance.RelativeY = 100f;
            }
            FanLeftInstance.ForceVelocityX = -200f;
            FanLeftInstance.ForceVelocityY = 0f;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp () 
        {
            CameraSetup.ResetCamera(SpriteManager.Camera);
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            FanRightInstance.RemoveFromManagers();
            FanLeftInstance.RemoveFromManagers();
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
                FanRightInstance.AssignCustomVariables(true);
                FanLeftInstance.AssignCustomVariables(true);
            }
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.X = -50f;
            }
            else
            {
                FanRightInstance.RelativeX = -50f;
            }
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.Y = -100f;
            }
            else
            {
                FanRightInstance.RelativeY = -100f;
            }
            if (FanRightInstance.Parent == null)
            {
                FanRightInstance.Z = 0f;
            }
            else
            {
                FanRightInstance.RelativeZ = 0f;
            }
            FanRightInstance.ForceVelocityX = 200f;
            FanRightInstance.ForceVelocityY = 0f;
            FanLeftInstance.TextInstanceDisplayText = "<- Fan";
            if (FanLeftInstance.Parent == null)
            {
                FanLeftInstance.X = 50f;
            }
            else
            {
                FanLeftInstance.RelativeX = 50f;
            }
            if (FanLeftInstance.Parent == null)
            {
                FanLeftInstance.Y = 100f;
            }
            else
            {
                FanLeftInstance.RelativeY = 100f;
            }
            FanLeftInstance.ForceVelocityX = -200f;
            FanLeftInstance.ForceVelocityY = 0f;
        }
        public virtual void ConvertToManuallyUpdated () 
        {
            FanRightInstance.ConvertToManuallyUpdated();
            FanLeftInstance.ConvertToManuallyUpdated();
        }
        public static void LoadStaticContent (string contentManagerName) 
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new System.ArgumentException("contentManagerName cannot be empty or null");
            }
            #if DEBUG
            if (contentManagerName == FlatRedBall.FlatRedBallServices.GlobalContentManager)
            {
                HasBeenLoadedWithGlobalContentManager = true;
            }
            else if (HasBeenLoadedWithGlobalContentManager)
            {
                throw new System.Exception("This type has been loaded with a Global content manager, then loaded with a non-global.  This can lead to a lot of bugs");
            }
            #endif
            Parme.Net.Frb.Example.Entities.FanRight.LoadStaticContent(contentManagerName);
            CustomLoadStaticContent(contentManagerName);
        }
        public override void PauseThisScreen () 
        {
            StateInterpolationPlugin.TweenerManager.Self.Pause();
            base.PauseThisScreen();
        }
        public override void UnpauseThisScreen () 
        {
            StateInterpolationPlugin.TweenerManager.Self.Unpause();
            base.UnpauseThisScreen();
        }
        [System.Obsolete("Use GetFile instead")]
        public static object GetStaticMember (string memberName) 
        {
            return null;
        }
        public static object GetFile (string memberName) 
        {
            return null;
        }
        object GetMember (string memberName) 
        {
            return null;
        }
        public static void Reload (object whatToReload) 
        {
        }
        partial void CustomActivityEditMode();
    }
}
