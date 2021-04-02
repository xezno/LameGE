using Engine;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Renderer.Managers;
using Engine.Utils;
using Engine.Utils.MathUtils;
using ExampleGame.Entities;
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

            SetupCustomImGuiMenus();
        }

        private void SetupCustomImGuiMenus()
        {
            // TODO: Replace with the new attribute system
            //ImGuiManager.Instance.Menus.Add(
            //    new ImGuiMenu(FontAwesome5.Hammer, ImGuiMenus.Menu.Experimental, new List<ImGuiWindow>()
            //    {
            //        new AnvilBrowserWindow()
            //    })
            //);
        }
    }
}
