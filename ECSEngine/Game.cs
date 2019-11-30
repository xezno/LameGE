using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Managers;

using OpenGL;
using OpenGL.CoreUI;

namespace ECSEngine
{
    public class Game : IHasParent
    {
        public IHasParent parent { get; set; }

        private DateTime lastUpdate;
        private readonly int titlebarHeight = 20; // TODO: I have no idea whether this is actually correct or not, but it works on win10.  Will need changing based on platform later
        private List<IManager> managers = new List<IManager>();
        private readonly Gl.DebugProc debugCallback; // Stored to prevent GC from collecting debug callback before it can be called

        public bool isRunning = true;

        public Game()
        {
            debugCallback = DebugCallback;
        }

        public void Run()
        {
            using NativeWindow nativeWindow = NativeWindow.Create();

            nativeWindow.ContextCreated += ContextCreated;
            nativeWindow.ContextDestroying += ContextDestroyed;
            nativeWindow.Render += Render;
            nativeWindow.KeyDown += KeyDown;
            nativeWindow.KeyUp += KeyUp;
            nativeWindow.MouseDown += MouseDown;
            nativeWindow.MouseUp += MouseUp;
            nativeWindow.MouseMove += MouseMove;
            nativeWindow.MouseWheel += MouseWheel;
            
            nativeWindow.CursorVisible = true; // Hide mouse cursor
            nativeWindow.Animation = false; // Changing this to true makes input poll like once every 500ms.  so don't change it
            nativeWindow.DepthBits = 24;
            nativeWindow.SwapInterval = 0;
            nativeWindow.Resize += Resize;
            
            nativeWindow.Create(0, 0, RenderSettings.Default.GameResolutionX, RenderSettings.Default.GameResolutionY, NativeWindowStyle.Caption);
            nativeWindow.Caption = "ECSEngine";

            // TODO: choice of monitor to use.

            nativeWindow.Show();
            nativeWindow.Run();
        }

        public void Render() { }

        public void Update(float deltaTime) { }
        
        private void Resize(object sender, EventArgs e)
        {
            var nativeWindow = (NativeWindow)sender;
            var windowSize = new Vector2(nativeWindow.ClientSize.Width, nativeWindow.ClientSize.Height);

            Gl.Viewport(0, 0, nativeWindow.ClientSize.Width, nativeWindow.ClientSize.Height);

            EventManager.BroadcastEvent(Event.WindowResized, new WindowResizeEventArgs(windowSize, this));
        }

        private void MouseWheel(object sender, NativeWindowMouseEventArgs e)
        {
            // TODO: Fix mouse wheel
            EventManager.BroadcastEvent(Event.MouseScroll, new MouseWheelEventArgs(e.WheelTicks, this));
        }

        private void MouseMove(object sender, NativeWindowMouseEventArgs e)
        {
            // For some reason this offsets by the titlebar height, and it's inverted, so we have to do some quick maths to fix that
            EventManager.BroadcastEvent(Event.MouseMove, 
                new MouseMoveEventArgs(new Vector2(
                    e.Location.X, RenderSettings.Default.GameResolutionY - e.Location.Y - titlebarHeight
                                  ), 
                    this)
                );
        }

        private void MouseUp(object sender, NativeWindowMouseEventArgs e)
        {
            int button = 0;
            if ((e.Buttons & MouseButton.Left) != 0)        button = 0;
            else if ((e.Buttons & MouseButton.Middle) != 0) button = 1;
            else if ((e.Buttons & MouseButton.Right) != 0)  button = 2;

            EventManager.BroadcastEvent(Event.MouseButtonUp, new MouseButtonEventArgs(button, this));
        }

        private void MouseDown(object sender, NativeWindowMouseEventArgs e)
        {
            int button = 0;
            if ((e.Buttons & MouseButton.Left) != 0)        button = 0;
            else if ((e.Buttons & MouseButton.Middle) != 0) button = 1;
            else if ((e.Buttons & MouseButton.Right) != 0)  button = 2;

            EventManager.BroadcastEvent(Event.MouseButtonDown, new MouseButtonEventArgs(button, this));
        }

        void KeyUp(object sender, NativeWindowKeyEventArgs e)
        {
            EventManager.BroadcastEvent(Event.KeyUp, new KeyboardEventArgs((int)e.Key, this));
        }

        void KeyDown(object sender, NativeWindowKeyEventArgs e)
        {
            EventManager.BroadcastEvent(Event.KeyDown, new KeyboardEventArgs((int)e.Key, this));
        }

        void ContextDestroyed(object sender, NativeWindowEventArgs e)
        {
            isRunning = false;
        }

        void DebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            if (severity >= DebugSeverity.DebugSeverityMedium)
                Debug.Log($"OpenGL Error {id}: {Marshal.PtrToStringAnsi(message, length)}", Debug.DebugSeverity.Fatal);
        }

        void SetUpSystems()
        {
            managers = new List<IManager>(){
                RenderManager.instance, // Ran first
                UpdateManager.instance,
                SceneManager.instance,
                ImGuiManager.instance
            };

            foreach (IManager manager in managers)
            {
                EventManager.AddManager(manager);
                manager.parent = this;
            }
        }

        void SetUpScene()
        {
            var entities = new List<IEntity>
            {
                new ShipEntity(),
                new TestModelEntity()
            };

            foreach (IEntity entity in entities)
                SceneManager.instance.AddEntity(entity);
        }

        void ContextCreated(object sender, NativeWindowEventArgs e)
        {
            NativeWindow nativeWindow = sender as NativeWindow;

            Debug.Log($"OpenGL {Gl.GetString(StringName.Version)}");
            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.ClearColor(100 / 255f, 149 / 255f, 237 / 255f, 1); // Cornflower blue (https://en.wikipedia.org/wiki/Web_colors#X11_color_names)
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.LineWidth(2.5f);

            Gl.DebugMessageCallback(debugCallback, IntPtr.Zero);

            SetUpSystems();
            SetUpScene();
            LoadContent();

            // Setup complete - broadcast the game started event
            EventManager.BroadcastEvent(Event.GameStart, new GenericEventArgs(this));
        }

        void LoadContent() { }

        void Render(object sender, NativeWindowEventArgs e)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float deltaTime = System.Math.Max((DateTime.Now - lastUpdate).Milliseconds, 1.0f) / 1000.0f;
            Debug.Log($"Delta time: {deltaTime}");
            foreach (IManager manager in managers)
            {
                manager.Run();
            }
            lastUpdate = DateTime.Now;
        }

        public T GetSystem<T>() where T : IManager
        {
            return (T)(managers.Find((system) => system.GetType() == typeof(T)));
        }
    }
}
