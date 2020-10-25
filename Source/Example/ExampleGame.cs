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
using System.Threading;
using LevelEditor;

#if LEVEL_EDITOR_ENABLED
using System.Windows.Forms;
#endif

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
                new LevelModelEntity(FileSystem.GetAsset("/Maps/gm_construct.bsp"))
                {
                    Name = "Generated BSP Mesh"
                },
                new ModelEntity(FileSystem.GetAsset("/Models/mcrn_tachi/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 0.0125f)
                {
                    Name = "MCRN Tachi"
                },
                new ModelEntity(FileSystem.GetAsset($"/Models/mimicgltf/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 5f)
                {
                    Name = "Treasure Chest Mimic"
                }
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);

            SetupCustomImGuiMenus();

            #if LEVEL_EDITOR_ENABLED
            SetupLevelEditorWindow();
            #endif
        }

        #if LEVEL_EDITOR_ENABLED
        private void SetupLevelEditorWindow()
        {
            var levelEditorThread = new Thread(LevelEditorThread);
            levelEditorThread.Start();
        }

        private void LevelEditorThread()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        #endif

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
