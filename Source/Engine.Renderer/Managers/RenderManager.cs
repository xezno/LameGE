using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.Base;
using System;
using System.Threading;

namespace Engine.Renderer.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        public string hdri = "/HDRIs/sapaced.hdr";
        public float exposure = 1.75f;
        public TonemapOperator tonemapOperator;

        public enum TonemapOperator
        {
            None,
            Reinhard,
            ReinhardExtendedLuminance,
            ReinhardJodie,
            AcesApproximation
        };

        private IRenderer renderer;

        private DateTime lastRender;
        private int currentFrametimeIndex;
        private int currentFramerateIndex;
        private const int FramesToCount = 480;
        private float framerateLimitAsMs = 1000f / GameSettings.FramerateLimit;
        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[FramesToCount];
        public float[] FramerateHistory { get; } = new float[FramesToCount];

        public bool RenderShadowMap { get; set; }
        public bool Paused { get; set; }

        #region Screens
        private const int MAX_SCREEN_STACK_SIZE = 16;
        private IScreen[] screens = new IScreen[MAX_SCREEN_STACK_SIZE];

        private int currentScreenStackPos = -1;

        private IScreen CurrentScreen => screens[currentScreenStackPos];
        #endregion

        public RenderManager()
        {
            renderer = ServiceLocator.Renderer;
            renderer.ContextCreated();
        }

        /// <summary>
        /// Render all the entities within the render manager.
        /// </summary>
        public override void Run()
        {
            if (currentScreenStackPos >= 0 && CurrentScreen != null)
            {
                CurrentScreen.Render();
            }

            SceneManager.Instance.RenderFramebuffer();

            CollectPerformanceData();
        }

        public void Update(float deltaTime)
        {
            if (currentScreenStackPos >= 0 && CurrentScreen != null)
            {
                CurrentScreen.Update(deltaTime);
            }
        }

        public void PushScreen(IScreen screen)
        {
            if (currentScreenStackPos >= 0 && CurrentScreen != null)
            {
                CurrentScreen.Exit();
            }

            screen.LoadContent();
            screen.Enter();

            screens[++currentScreenStackPos] = screen;
        }

        public IScreen PopScreen()
        {
            CurrentScreen.Exit();
            CurrentScreen.UnloadContent();

            return screens[--currentScreenStackPos];
        }

        public IScreen PeekScreen()
        {
            return screens[currentScreenStackPos];
        }

        public void CollectPerformanceData()
        {
            LastFrameTime = (DateTime.Now - lastRender).Milliseconds;

            if (!Paused)
            {
                FrametimeHistory[currentFrametimeIndex++] = LastFrameTime;

                if (currentFrametimeIndex == FrametimeHistory.Length)
                {
                    currentFrametimeIndex--;
                    for (var i = 0; i < FrametimeHistory.Length; ++i)
                        FrametimeHistory[i] = FrametimeHistory[(i + 1) % FrametimeHistory.Length];
                }

                FramerateHistory[currentFramerateIndex++] = CalculatedFramerate;
                if (currentFramerateIndex == FramerateHistory.Length)
                {
                    currentFramerateIndex--;
                    for (var i = 0; i < FramerateHistory.Length; ++i)
                        FramerateHistory[i] = FramerateHistory[(i + 1) % FramerateHistory.Length];
                }
            }

            lastRender = DateTime.Now;

            // Slow down rendering if it's going past the framerate limit
            if (LastFrameTime < framerateLimitAsMs && GameSettings.FramerateLimit > 0)
            {
                var nextFrameDelay = (int)Math.Ceiling(framerateLimitAsMs - LastFrameTime);
                Thread.Sleep(nextFrameDelay);
            }
        }
    }
}
