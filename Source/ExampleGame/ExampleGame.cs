using Engine;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils;
using Engine.Utils.MathUtils;
using ExampleGame.Entities;
using ExampleGame.Screens;
using System;
using System.Collections.Generic;

namespace ExampleGame
{
    internal sealed class ExampleGame : Game
    {
        public ExampleGame(string gamePropertyPath) : base(gamePropertyPath) { }
        
        protected override void InitScene()
        {
            base.InitScene();

            RenderManager.Instance.PushScreen(new LoadingScreen(new SceneScreen()));
        }
    }
}
