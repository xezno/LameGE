using Engine;
using Engine.Assets;
using Engine.ECS.Entities;
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
            base.InitScene();
            var entities = new List<IEntity>
            {
                new PlayerEntity()
                {
                    Name = "Player"
                },
                new LevelModelEntity("Content/Maps/gm_flatgrass.bsp")
                {
                    Name = "Generated BSP Mesh"
                },
                //new TestCubeEntity()
                //{
                //    Name = "Physics Box"
                //},
                //new ModelEntity($"Content/Models/rainier/scene.gltf", new Vector3d(-10f, 0, 0), modelScale)
                //{
                //    Name = "Rainier"
                //},
                new ModelEntity($"Content/Models/mcrn_tachi/scene.gltf", new Vector3d(0, 0, 0), Vector3d.one * 0.0125f)
                {
                    Name = "MCRN Tachi"
                },
                //new ModelEntity($"Content/Models/mimicgltf/scene.gltf", new Vector3d(0, 0, 0), Vector3d.one * 5f)
                //{
                //    Name = "Treasure Chest Mimic"
                //}
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
