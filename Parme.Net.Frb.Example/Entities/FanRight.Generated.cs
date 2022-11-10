#if ANDROID || IOS || DESKTOP_GL
#define REQUIRES_PRIMARY_THREAD_LOADING
#endif
#define SUPPORTS_GLUEVIEW_2
using Color = Microsoft.Xna.Framework.Color;
using System.Linq;
using FlatRedBall.Graphics;
using FlatRedBall.Math;
using FlatRedBall;
using System;
using System.Collections.Generic;
using System.Text;
namespace Parme.Net.Frb.Example.Entities
{
    public partial class FanRight : FlatRedBall.PositionedObject, FlatRedBall.Graphics.IDestroyable, FlatRedBall.Entities.IEntity
    {
        // This is made static so that static lazy-loaded content can access it.
        public static string ContentManagerName { get; set; }
        #if DEBUG
        static bool HasBeenLoadedWithGlobalContentManager = false;
        #endif
        static object mLockObject = new object();
        static System.Collections.Generic.List<string> mRegisteredUnloads = new System.Collections.Generic.List<string>();
        static System.Collections.Generic.List<string> LoadedContentManagers = new System.Collections.Generic.List<string>();
        
        private FlatRedBall.Graphics.Text TextInstance;
        private FlatRedBall.Math.Geometry.AxisAlignedRectangle mForceArea;
        public FlatRedBall.Math.Geometry.AxisAlignedRectangle ForceArea
        {
            get
            {
                return mForceArea;
            }
            private set
            {
                mForceArea = value;
            }
        }
        public float ForceVelocityX;
        public float ForceVelocityY;
        public string TextInstanceDisplayText
        {
            get
            {
                return TextInstance.DisplayText;
            }
            set
            {
                TextInstance.DisplayText = value;
            }
        }
        public float ForceAreaWidth
        {
            get
            {
                return ForceArea.Width;
            }
            set
            {
                ForceArea.Width = value;
            }
        }
        public float ForceAreaHeight
        {
            get
            {
                return ForceArea.Height;
            }
            set
            {
                ForceArea.Height = value;
            }
        }
        protected FlatRedBall.Graphics.Layer LayerProvidedByContainer = null;
        public FanRight () 
        	: this(FlatRedBall.Screens.ScreenManager.CurrentScreen.ContentManagerName, true)
        {
        }
        public FanRight (string contentManagerName) 
        	: this(contentManagerName, true)
        {
        }
        public FanRight (string contentManagerName, bool addToManagers) 
        	: base()
        {
            ContentManagerName = contentManagerName;
            InitializeEntity(addToManagers);
        }
        protected virtual void InitializeEntity (bool addToManagers) 
        {
            LoadStaticContent(ContentManagerName);
            TextInstance = new FlatRedBall.Graphics.Text();
            TextInstance.Name = "TextInstance";
            TextInstance.CreationSource = "Glue";
            mForceArea = new FlatRedBall.Math.Geometry.AxisAlignedRectangle();
            mForceArea.Name = "ForceArea";
            mForceArea.CreationSource = "Glue";
            
            PostInitialize();
            if (addToManagers)
            {
                AddToManagers(null);
            }
        }
        public virtual void ReAddToManagers (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            LayerProvidedByContainer = layerToAddTo;
            FlatRedBall.SpriteManager.AddPositionedObject(this);
            FlatRedBall.Graphics.TextManager.AddToLayer(TextInstance, LayerProvidedByContainer);
            if (TextInstance.Font != null)
            {
                TextInstance.SetPixelPerfectScale(LayerProvidedByContainer);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mForceArea, LayerProvidedByContainer);
        }
        public virtual void AddToManagers (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            LayerProvidedByContainer = layerToAddTo;
            FlatRedBall.SpriteManager.AddPositionedObject(this);
            FlatRedBall.Graphics.TextManager.AddToLayer(TextInstance, LayerProvidedByContainer);
            if (TextInstance.Font != null)
            {
                TextInstance.SetPixelPerfectScale(LayerProvidedByContainer);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(mForceArea, LayerProvidedByContainer);
            AddToManagersBottomUp(layerToAddTo);
            CustomInitialize();
        }
        public virtual void Activity () 
        {
            
            CustomActivity();
        }
        public virtual void ActivityEditMode () 
        {
            CustomActivityEditMode();
        }
        public virtual void Destroy () 
        {
            FlatRedBall.SpriteManager.RemovePositionedObject(this);
            
            if (TextInstance != null)
            {
                FlatRedBall.Graphics.TextManager.RemoveTextOneWay(TextInstance);
            }
            if (ForceArea != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ForceArea);
            }
            CustomDestroy();
        }
        public virtual void PostInitialize () 
        {
            bool oldShapeManagerSuppressAdd = FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = true;
            if (TextInstance.Parent == null)
            {
                TextInstance.CopyAbsoluteToRelative();
                TextInstance.AttachTo(this, false);
            }
            TextInstance.DisplayText = "Fan->";
            if (mForceArea.Parent == null)
            {
                mForceArea.CopyAbsoluteToRelative();
                mForceArea.AttachTo(this, false);
            }
            if (ForceArea.Parent == null)
            {
                ForceArea.X = 0f;
            }
            else
            {
                ForceArea.RelativeX = 0f;
            }
            ForceArea.Width = 200f;
            ForceArea.Height = 100f;
            FlatRedBall.Math.Geometry.ShapeManager.SuppressAddingOnVisibilityTrue = oldShapeManagerSuppressAdd;
        }
        public virtual void AddToManagersBottomUp (FlatRedBall.Graphics.Layer layerToAddTo) 
        {
            AssignCustomVariables(false);
        }
        public virtual void RemoveFromManagers () 
        {
            FlatRedBall.SpriteManager.ConvertToManuallyUpdated(this);
            if (TextInstance != null)
            {
                FlatRedBall.Graphics.TextManager.RemoveTextOneWay(TextInstance);
            }
            if (ForceArea != null)
            {
                FlatRedBall.Math.Geometry.ShapeManager.RemoveOneWay(ForceArea);
            }
        }
        public virtual void AssignCustomVariables (bool callOnContainedElements) 
        {
            if (callOnContainedElements)
            {
            }
            TextInstance.DisplayText = "Fan->";
            if (ForceArea.Parent == null)
            {
                ForceArea.X = 0f;
            }
            else
            {
                ForceArea.RelativeX = 0f;
            }
            ForceArea.Width = 200f;
            ForceArea.Height = 100f;
            TextInstanceDisplayText = "Fan->";
            ForceAreaWidth = 200f;
            ForceAreaHeight = 100f;
        }
        public virtual void ConvertToManuallyUpdated () 
        {
            this.ForceUpdateDependenciesDeep();
            FlatRedBall.SpriteManager.ConvertToManuallyUpdated(this);
            FlatRedBall.Graphics.TextManager.ConvertToManuallyUpdated(TextInstance);
        }
        public static void LoadStaticContent (string contentManagerName) 
        {
            if (string.IsNullOrEmpty(contentManagerName))
            {
                throw new System.ArgumentException("contentManagerName cannot be empty or null");
            }
            ContentManagerName = contentManagerName;
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
            bool registerUnload = false;
            if (LoadedContentManagers.Contains(contentManagerName) == false)
            {
                LoadedContentManagers.Add(contentManagerName);
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBall.FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("FanRightStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            if (registerUnload && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
            {
                lock (mLockObject)
                {
                    if (!mRegisteredUnloads.Contains(ContentManagerName) && ContentManagerName != FlatRedBall.FlatRedBallServices.GlobalContentManager)
                    {
                        FlatRedBall.FlatRedBallServices.GetContentManagerByName(ContentManagerName).AddUnloadMethod("FanRightStaticUnload", UnloadStaticContent);
                        mRegisteredUnloads.Add(ContentManagerName);
                    }
                }
            }
            CustomLoadStaticContent(contentManagerName);
        }
        public static void UnloadStaticContent () 
        {
            if (LoadedContentManagers.Count != 0)
            {
                LoadedContentManagers.RemoveAt(0);
                mRegisteredUnloads.RemoveAt(0);
            }
            if (LoadedContentManagers.Count == 0)
            {
            }
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
        protected bool mIsPaused;
        public override void Pause (FlatRedBall.Instructions.InstructionList instructions) 
        {
            base.Pause(instructions);
            mIsPaused = true;
        }
        public virtual void SetToIgnorePausing () 
        {
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(this);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(TextInstance);
            FlatRedBall.Instructions.InstructionManager.IgnorePausingFor(ForceArea);
        }
        public virtual void MoveToLayer (FlatRedBall.Graphics.Layer layerToMoveTo) 
        {
            var layerToRemoveFrom = LayerProvidedByContainer;
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(TextInstance);
            }
            if (layerToMoveTo != null || !TextManager.AutomaticallyUpdatedTexts.Contains(TextInstance))
            {
                FlatRedBall.Graphics.TextManager.AddToLayer(TextInstance, layerToMoveTo);
            }
            if (layerToRemoveFrom != null)
            {
                layerToRemoveFrom.Remove(ForceArea);
            }
            FlatRedBall.Math.Geometry.ShapeManager.AddToLayer(ForceArea, layerToMoveTo);
            LayerProvidedByContainer = layerToMoveTo;
        }
        partial void CustomActivityEditMode();
    }
}
