using System.Collections.Generic;

using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Systems;

using OpenGL;
using OpenGL.CoreUI;

namespace ECSEngine
{
    public class Game
    {
        public bool isRunning { get => true; }

        private WorldSystem worldSystem;
        public Game()
        {
            using (NativeWindow nativeWindow = NativeWindow.Create())
            {
                nativeWindow.ContextCreated += ContextCreated;
                nativeWindow.Render += Render;
                nativeWindow.Animation = true;

                nativeWindow.Create(0, 0, RenderSettings.Default.GameResolutionX, RenderSettings.Default.GameResolutionY, NativeWindowStyle.Border);

                nativeWindow.Show();
                nativeWindow.Run();
            }
        }

        private void SetUpSystems()
        {
            worldSystem = new WorldSystem()
            {
                entities = new List<IEntity>()
                {
                    new TestModelEntity()
                }
            };

            EventManager.RegisterWorldSystem(worldSystem);
        }

        private void ContextCreated(object sender, NativeWindowEventArgs e)
        {
            NativeWindow nativeWindow = sender as NativeWindow;

            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
            Gl.Enable(EnableCap.Blend | EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.LineWidth(2.5f);

            SetUpSystems();

            // All done - broadcast the game started event
            EventManager.BroadcastEvent(Event.GameStartEvent, new GenericEventArgs(this));
        }

        private void Render(object sender, NativeWindowEventArgs e)
        {
            Gl.Viewport(0, 0, (int)RenderSettings.Default.GameResolutionX, (int)RenderSettings.Default.GameResolutionY);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }
    }
}
