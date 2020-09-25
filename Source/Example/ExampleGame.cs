using Engine;
using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Entities;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows;
using System.Collections.Generic;
using Example.Entities;
using Example.Managers.ImGuiWindows.Addons;
using Quincy.Managers;
using Engine.Utils.MathUtils;

namespace Example
{
    internal sealed class ExampleGame : Game
    {
        public ExampleGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            var modelScaleFactor = 0.0125f;
            var modelScale = new Vector3d(modelScaleFactor, modelScaleFactor, modelScaleFactor);
            base.InitScene();
            var entities = new List<IEntity>
            {
                new PlayerEntity()
                {
                    Name = "Player"
                },
                //new LevelModelEntity()
                //{
                //    Name = "Generated BSP Mesh"
                //},
                //new TestCubeEntity()
                //{
                //    Name = "Physics Box"
                //},
                new ModelEntity($"Content/Models/rainier/scene.gltf", modelScale)
                {
                    Name = "Rainier"
                },
                new ModelEntity($"Content/Models/mcrn_tachi/scene.gltf", modelScale)
                {
                    Name = "MCRN Tachi"
                }
                // TODO: fix this (anything that renders afterwards won't render at all?)
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);

            SetupCustomImGuiMenus();
        }

        private void SetupCustomImGuiMenus()
        {
            ImGuiManager.Instance.Menus.Add(
                new ImGuiMenu(FontAwesome5.Hammer, "Anvil", new List<ImGuiWindow>()
                {
                    new AnvilBrowserWindow()
                })
            );
        }
    }
}
