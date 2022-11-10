using System.Linq;
namespace Parme.Net.Frb.Example
{
    public partial class Game1
    {
        GlueCommunication.GameConnectionManager gameConnectionManager;
        GlueControl.GlueControlManager glueControlManager;
        partial void GeneratedInitialize () 
        {
            System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += (s, e) =>
            {
                // Get just the name of assmebly
                // Aseembly name excluding version and other metadata
                string name = e.Name.Contains(", ") ? e.Name.Substring(0, e.Name.IndexOf(", ")) : e.Name;
            
                if (name == "Newtonsoft.Json")
                {
                    // Load whatever version available
                    return System.Reflection.Assembly.Load(name);
                }
            
                return null;
            };
            
            gameConnectionManager = new GlueCommunication.GameConnectionManager(8785);
            gameConnectionManager.OnPacketReceived += async (packet) =>
            {
                if (packet.Packet.PacketType == "OldDTO")
                {
                    var returnValue = await glueControlManager.ProcessMessage(packet.Packet.Payload);
            
                    gameConnectionManager.SendItem(new GlueCommunication.GameConnectionManager.Packet
                    {
                        PacketType = "OldDTO",
                        Payload = returnValue,
                        InResponseTo = packet.Packet.Id
                    });
                }
            };
            this.Exiting += (not, used) => gameConnectionManager.Dispose();
            var args = System.Environment.GetCommandLineArgs();
            bool? changeResize = null;
            var resizeArgs = args.FirstOrDefault(item => item.StartsWith("AllowWindowResizing="));
            if (!string.IsNullOrEmpty(resizeArgs))
            {
                var afterEqual = resizeArgs.Split('=')[1];
                changeResize = bool.Parse(afterEqual);
            }
            if (changeResize != null)
            {
                CameraSetup.Data.AllowWindowResizing = changeResize.Value;
            }
            CameraSetup.SetupCamera(FlatRedBall.Camera.Main, graphics);
            glueControlManager = new GlueControl.GlueControlManager(8785);
            FlatRedBall.FlatRedBallServices.GraphicsOptions.SizeOrOrientationChanged += (not, used) =>
            {
                if (FlatRedBall.Screens.ScreenManager.IsInEditMode)
                {
                    GlueControl.Editing.CameraLogic.UpdateCameraToZoomLevel(zoomAroundCursorPosition: false);
                }
                GlueControl.Editing.CameraLogic.PushZoomLevelToEditor();
            }
            ;
            FlatRedBall.Screens.ScreenManager.BeforeScreenCustomInitialize += (newScreen) => 
            {
                glueControlManager.ReRunAllGlueToGameCommands();
                // These get nulled out when screens are destroyed so we have to re-assign them
                Factories.FanRightFactory.EntitySpawned += (newEntity) =>  GlueControl.InstanceLogic.Self.ApplyEditorCommandsToNewEntity(newEntity);
            }
            ;
            FlatRedBall.Screens.ScreenManager.AfterScreenDestroyed += (screen) =>
            {
                GlueControl.Editing.EditorVisuals.DestroyContainedObjects();
            }
            ;
            System.Type startScreenType = typeof(Parme.Net.Frb.Example.Screens.ForceCollisionDemoScreen);
            var commandLineArgs = System.Environment.GetCommandLineArgs();
            if (commandLineArgs.Length > 0)
            {
                var thisAssembly = this.GetType().Assembly;
                // see if any of these are screens:
                foreach (var item in commandLineArgs)
                {
                    var type = thisAssembly.GetType(item);
                    if (type != null)
                    {
                        startScreenType = type;
                        break;
                    }
                }
            }
            if (startScreenType != null)
            {
                FlatRedBall.Screens.ScreenManager.Start(startScreenType);
            }
        }
        partial void GeneratedUpdate (Microsoft.Xna.Framework.GameTime gameTime) 
        {
        }
        partial void GeneratedDraw (Microsoft.Xna.Framework.GameTime gameTime) 
        {
        }
    }
}
