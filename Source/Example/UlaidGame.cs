using Engine;
using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Entities;
using Engine.Gui.Managers;
using Engine.Gui.Managers.ImGuiWindows;
using Engine.Renderer.GL.Entities;
using Engine.Renderer.GL.Managers;
using System.Collections.Generic;
using Example.Entities;
using Example.Managers.ImGuiWindows.Addons;

namespace Example
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
                //new TestCubeEntity()
                //{
                //    Name = "Physics Box"
                //},
                // TODO: fix this (anything that renders afterwards won't render at all?)
                new HudEntity()
                {
                    Name = "CEF HUD Entity"
                },
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
