using Engine.Renderer;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Renderer.Primitives;
using Engine.Utils;
using ImGuiNET;
using OpenGL;
using System;

namespace ExampleGame.Screens
{
    class LoadingScreen : IScreen
    {
        private IScreen targetScreen;
        private bool hasRendered = false;
        private bool hasLoaded = false;

        public LoadingScreen(IScreen target)
        {
            // Handles loading for target screen
            targetScreen = target;
        }

        public void Enter()
        { }

        public void Exit()
        { }

        public void LoadContent()
        { }

        public void Render()
        {
            if (hasRendered && !hasLoaded)
            {
                // Load on frame AFTER rendering
                RenderManager.Instance.PushScreen(targetScreen);
                hasLoaded = true;
            }
            else if (!hasRendered)
            {
                // Show something so that we know the game is working
                // This would ideally be something non-static and would ideally look like a progress bar or something,
                // but OpenGL multi-threading sucks and I can't be bothered to implement it when I plan on moving
                // over to Veldrid anyways.
                SceneManager.Instance.RenderShadows();
                SceneManager.Instance.Render();
                hasRendered = true;
            }
        }

        public void UnloadContent()
        { }

        public void Update(float deltaTime)
        { }
    }
}
