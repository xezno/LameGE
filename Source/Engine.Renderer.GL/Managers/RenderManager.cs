﻿using Engine.ECS.Managers;
using Engine.Utils;
using System;
using System.Threading;

namespace Engine.Renderer.GL.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        private DateTime lastRender;
        private int currentFrametimeIndex;
        private int currentFramerateIndex;

        private const int FramesToCount = 120;

        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[FramesToCount];
        public float[] FramerateHistory { get; } = new float[FramesToCount];

        public RenderManager() { }

        /// <summary>
        /// Render all the entities within the render manager.
        /// </summary>
        public override void Run()
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled)
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
            if (LastFrameTime < (1000f / GameSettings.FramerateLimit) && GameSettings.FramerateLimit > 0)
            {
                // really crappy implementation
                // TODO: do this differently
                Thread.Sleep((int)Math.Ceiling((1000f / GameSettings.FramerateLimit) - LastFrameTime));
            }
        }
    }
}