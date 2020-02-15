using ECSEngine.Entities;
using System;
using System.Threading;

namespace ECSEngine.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        private DateTime lastRender;
        private Random random = new Random();
        public float lastFrameTime { get; private set; }
        public int calculatedFramerate => (int)(1000f / Math.Max(lastFrameTime, 0.001f));
        public float[] frametimeHistory { get; private set; } = new float[100];
        public float[] framerateHistory { get; private set; } = new float[100];
        private int currentFrametimeIndex;
        private int currentFramerateIndex;
        public bool fakeLag = false;

        /// <summary>
        /// Render all the entities within the world manager.
        /// </summary>
        public override void Run()
        {
            foreach (IEntity entity in SceneManager.instance.entities)
            {
                entity.Render();
            }

            lastFrameTime = (DateTime.Now - lastRender).Milliseconds;
            frametimeHistory[currentFrametimeIndex++] = lastFrameTime;
            if (currentFrametimeIndex == frametimeHistory.Length)
            {
                currentFrametimeIndex--;
                for (int i = 0; i < frametimeHistory.Length; ++i)
                    frametimeHistory[i] = frametimeHistory[(i + 1) % frametimeHistory.Length];
            }


            framerateHistory[currentFramerateIndex++] = calculatedFramerate;
            if (currentFramerateIndex == framerateHistory.Length)
            {
                currentFramerateIndex--;
                for (int i = 0; i < framerateHistory.Length; ++i)
                    framerateHistory[i] = framerateHistory[(i + 1) % framerateHistory.Length];
            }

            lastRender = DateTime.Now;
            if (fakeLag)
                Thread.Sleep(random.Next(16, 300));
        }
    }
}
