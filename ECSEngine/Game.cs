using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Managers;
using ECSEngine.Math;
using Newtonsoft.Json;
using OpenGL;
using OpenGL.CoreUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ECSEngine
{
    public class Game : IHasParent
    {
        private DateTime lastUpdate;
        private readonly int titlebarHeight = 20; // TODO: I have no idea whether this is actually correct or not, but it works on win10.  Will need changing based on platform later
        private List<IManager> managers = new List<IManager>();
        private readonly Gl.DebugProc debugCallback; // Stored to prevent GC from collecting debug callback before it can be called
        private string gamePropertyPath;
        private GameProperties gameProperties;

        protected NativeWindow nativeWindow;

        public IHasParent parent { get; set; }
        public bool isRunning = true;

        public Game(string gamePropertyPath)
        {
            this.gamePropertyPath = gamePropertyPath;
            debugCallback = DebugCallback;
        }

        public void Run()
        {
            LoadGameProperties();
            InitNativeWindow();
        }

        #region Initialization
        private void InitNativeWindow()
        {
            nativeWindow = NativeWindow.Create();

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

            nativeWindow.Caption = FilterString(gameProperties.windowTitle) ?? "ECSEngine";

            // TODO: get choice of monitor to use.

            nativeWindow.Show();
            nativeWindow.Run();
            nativeWindow.Destroy();
        }

        private string FilterString(string str)
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            str = str.Replace("{Version}", version?.ToString())
                .Replace("{Build}", version?.Build.ToString())
                .Replace("{Revision}", version?.Revision.ToString());
            return str;
        }

        private void LoadGameProperties()
        {
            using var streamReader = new StreamReader(gamePropertyPath);
            gameProperties = JsonConvert.DeserializeObject<GameProperties>(streamReader.ReadToEnd());
        }

        private void InitSystems()
        {
            managers = new List<IManager> {
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

        private void InitScene()
        {
            var entities = new List<IEntity>
            {
                new ShipEntity(),
                new TestModelEntity()
            };

            foreach (IEntity entity in entities)
                SceneManager.instance.AddEntity(entity);
        }

        private void LoadContent() { }
        #endregion

        #region Event Handlers
        private void ContextCreated(object sender, NativeWindowEventArgs e)
        {
            Debug.Log($"OpenGL {Gl.GetString(StringName.Version)}");
            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.ClearColor(100 / 255f, 149 / 255f, 237 / 255f, 1); // Cornflower blue (https://en.wikipedia.org/wiki/Web_colors#X11_color_names)
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.LineWidth(2.5f);

            Gl.DebugMessageCallback(debugCallback, IntPtr.Zero);

            InitSystems();
            InitScene();
            LoadContent();

            // Setup complete - broadcast the game started event
            EventManager.BroadcastEvent(Event.GameStart, new GenericEventArgs(this));
        }

        private void Resize(object sender, EventArgs e)
        {
            var nativeWindow = (NativeWindow)sender;
            var windowSize = new Vector2(nativeWindow.ClientSize.Width, nativeWindow.ClientSize.Height);

            Gl.Viewport(0, 0, nativeWindow.ClientSize.Width, nativeWindow.ClientSize.Height);

            EventManager.BroadcastEvent(Event.WindowResized, new WindowResizeEventArgs(windowSize, this));
        }

        // TODO: Fix mouse wheel
        private void MouseWheel(object sender, NativeWindowMouseEventArgs e) => EventManager.BroadcastEvent(Event.MouseScroll, new MouseWheelEventArgs(e.WheelTicks, this));

        // For some reason this offsets by the titlebar height, and it's inverted, so we have to do some quick maths to fix that
        private void MouseMove(object sender, NativeWindowMouseEventArgs e) =>
            EventManager.BroadcastEvent(Event.MouseMove,
                new MouseMoveEventArgs(new Vector2(
                    e.Location.X, RenderSettings.Default.GameResolutionY - e.Location.Y - titlebarHeight
                                  ),
                    this)
                );

        private void MouseUp(object sender, NativeWindowMouseEventArgs e)
        {
            int button = 0;
            if ((e.Buttons & MouseButton.Left) != 0) button = 0;
            else if ((e.Buttons & MouseButton.Middle) != 0) button = 1;
            else if ((e.Buttons & MouseButton.Right) != 0) button = 2;

            EventManager.BroadcastEvent(Event.MouseButtonUp, new MouseButtonEventArgs(button, this));
        }

        private void MouseDown(object sender, NativeWindowMouseEventArgs e)
        {
            int button = 0;
            if ((e.Buttons & MouseButton.Left) != 0) button = 0;
            else if ((e.Buttons & MouseButton.Middle) != 0) button = 1;
            else if ((e.Buttons & MouseButton.Right) != 0) button = 2;

            EventManager.BroadcastEvent(Event.MouseButtonDown, new MouseButtonEventArgs(button, this));
        }

        private void KeyUp(object sender, NativeWindowKeyEventArgs e) => EventManager.BroadcastEvent(Event.KeyUp, new KeyboardEventArgs((int)e.Key, this));

        private void KeyDown(object sender, NativeWindowKeyEventArgs e) => EventManager.BroadcastEvent(Event.KeyDown, new KeyboardEventArgs((int)e.Key, this));

        private void ContextDestroyed(object sender, NativeWindowEventArgs e)
        {
            isRunning = false;
        }

        private void DebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            if (severity >= DebugSeverity.DebugSeverityMedium)
                Debug.Log($"OpenGL Error {id}: {Marshal.PtrToStringAnsi(message, length)}", Debug.DebugSeverity.Fatal);
        }
        #endregion

        private void Render(object sender, NativeWindowEventArgs e)
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
    }
}
