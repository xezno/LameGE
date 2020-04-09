using System;
using System.Threading;

namespace ECSEngine.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        private DateTime lastRender;
        private Random random = new Random();
        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[1000];
        public float[] FramerateHistory { get; } = new float[1000];
        private int currentFrametimeIndex;
        private int currentFramerateIndex;

        /// <summary>
        /// Render all the entities within the world manager.
        /// </summary>
        public override void Run()
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                entity.Render();
            }

            LastFrameTime = (DateTime.Now - lastRender).Milliseconds;
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

            lastRender = DateTime.Now;

            // Are we rendering too fast?
            if (LastFrameTime < (1000f / RenderSettings.Default.framerateLimit) && RenderSettings.Default.framerateLimit > 0)
            {
                // really crappy implementation
                // TODO: do this differently
                Thread.Sleep((int)Math.Ceiling((1000f / RenderSettings.Default.framerateLimit) - LastFrameTime));
            }
        }
    }
}
