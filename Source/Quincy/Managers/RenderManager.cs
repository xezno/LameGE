using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Threading;

namespace Quincy.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        private DateTime lastRender;
        private int currentFrametimeIndex;
        private int currentFramerateIndex;

        private const int FramesToCount = 480;
        private readonly QuincyRenderer renderer;

        private float framerateLimitAsMs = 1000f / GameSettings.FramerateLimit;

        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[FramesToCount];
        public float[] FramerateHistory { get; } = new float[FramesToCount];

        public bool RenderShadowMap { get; set; }
        public bool Paused { get; set; }

        public RenderManager()
        {
            renderer = new QuincyRenderer();
            renderer.ContextCreated();
        }

        /// <summary>
        /// Render all the entities within the render manager.
        /// </summary>
        public override void Run()
        {
            renderer.RenderToShadowMap();
            renderer.RenderToScreen();
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
