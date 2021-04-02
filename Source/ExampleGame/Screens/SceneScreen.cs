﻿using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Renderer;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils;
using Engine.Utils.MathUtils;
using ExampleGame.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExampleGame.Screens
{
    class SceneScreen : IScreen
    {
        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void LoadContent()
        {
            var fs = ServiceLocator.FileSystem;

            var playerEntity =
                new PlayerEntity()
                {
                    Name = "Player"
                };

            var mcrnModel =
                new ModelEntity(fs.GetAsset("/Models/mcrn_tachi/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 0.0125f)
                {
                    Name = "MCRN Tachi"
                };

            var level = new LevelModelEntity(fs.GetAsset("/Maps/gm_flatgrass.bsp"));

            mcrnModel.GetComponent<TransformComponent>().ParentTransform = playerEntity.GetComponent<TransformComponent>();

            var entities = new List<IEntity>
            {
                playerEntity,
                mcrnModel,
                level
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);

            Subject.Notify(NotifyType.ContextReady, new GenericNotifyArgs(this));
        }

        public void Render()
        {
            SceneManager.Instance.RenderShadows();
            SceneManager.Instance.Render();
        }

        public void UnloadContent()
        {
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled)
                    entity.Update(deltaTime);
            }
        }
    }
}
