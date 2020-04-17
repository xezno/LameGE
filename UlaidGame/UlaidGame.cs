﻿using ECSEngine;
using ECSEngine.Entities;
using ECSEngine.Managers;
using System.Collections.Generic;
using UlaidGame.Entities;

namespace UlaidGame
{
    internal sealed class UlaidGame : Game
    {
        public UlaidGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
            var entities = new List<IEntity>
            {
                new PlayerEntity()
                {
                    Name = "Player"
                },
                new SkyboxEntity()
                {
                    Name = "Skybox"
                },
                new LevelModelEntity()
                {
                    Name = "Generated BSP Mesh"
                },
                // TODO: fix this (anything that renders afterwards won't render at all?)
                new CefEntity()
                {
                    Name = "CEF HUD Entity"
                }
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);
        }
    }
}
