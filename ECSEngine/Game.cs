using System;
using System.Runtime.InteropServices;

using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Systems;

using OpenGL;
using OpenGL.CoreUI;

namespace ECSEngine
{
    public class Game
    {
        WorldSystem worldSystem;
        Gl.DebugProc debugCallback; // Stored to prevent GC from collecting debug callback before it can be called

        public bool isRunning = true;

        public Game()
        {
            debugCallback = DebugCallback;
            using (NativeWindow nativeWindow = NativeWindow.Create())
            {
                nativeWindow.ContextCreated += ContextCreated;
                nativeWindow.Render += Render;
                nativeWindow.KeyDown += KeyDown;
                nativeWindow.KeyUp += KeyUp;
                nativeWindow.ContextDestroying += ContextDestroyed;
                nativeWindow.Animation = true;
                nativeWindow.DepthBits = 24;

                nativeWindow.Create(0, 0, RenderSettings.Default.GameResolutionX, RenderSettings.Default.GameResolutionY, NativeWindowStyle.Overlapped);

                nativeWindow.Show();
                nativeWindow.Run();
            }
        }

        void ContextDestroyed(object sender, NativeWindowEventArgs e)
        {
            isRunning = false;
        }

        void DebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            Debug.Log($"OpenGL Error {id}: {Marshal.PtrToStringAnsi(message, length)}", Debug.DebugSeverity.Fatal);
        }

        void KeyUp(object sender, NativeWindowKeyEventArgs e)
        {
            EventManager.BroadcastEvent(Event.KeyUp, new GenericEventArgs(this));
        }

        void KeyDown(object sender, NativeWindowKeyEventArgs e)
        {
            EventManager.BroadcastEvent(Event.KeyDown, new GenericEventArgs(this));
        }

        void SetUpSystems()
        {
            worldSystem = new WorldSystem();
            worldSystem.AddEntity(new TestModelEntity());

            EventManager.RegisterWorldSystem(worldSystem);
        }

        void ContextCreated(object sender, NativeWindowEventArgs e)
        {
            NativeWindow nativeWindow = sender as NativeWindow;

            Debug.Log($"OpenGL {Gl.GetString(StringName.Version)}");
            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.ClearColor(1f, 1f, 1f, 1f);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.LineWidth(2.5f);

            Gl.DebugMessageCallback(debugCallback, IntPtr.Zero);

            SetUpSystems();
            LoadContent();

            // Setup complete - broadcast the game started event
            EventManager.BroadcastEvent(Event.GameStart, new GenericEventArgs(this));
        }

        void LoadContent() { }

        void Render(object sender, NativeWindowEventArgs e)
        {
            Gl.Viewport(0, 0, (int)RenderSettings.Default.GameResolutionX, (int)RenderSettings.Default.GameResolutionY);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            worldSystem.Render();
        }
    }
}
