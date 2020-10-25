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
using Engine.Utils.FileUtils;
using Engine.Utils;

namespace Example
{
    internal sealed class ExampleGame : Game
    {
        public ExampleGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
            var fs = ServiceLocator.fileSystem.GetService();

            var entities = new List<IEntity>
            {
                new PlayerEntity()
                {
                    Name = "Player"
                },
                new LevelModelEntity(fs.GetAsset("/Maps/gm_fork.bsp"))
                {
                    Name = "Generated BSP Mesh"
                },
                new ModelEntity(fs.GetAsset("/Models/mcrn_tachi/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 0.0125f)
                {
                    Name = "MCRN Tachi"
                },
                new ModelEntity(fs.GetAsset("/Models/mimicgltf/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 5f)
                {
                    Name = "Treasure Chest Mimic"
                }
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
