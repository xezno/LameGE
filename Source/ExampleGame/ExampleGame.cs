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
            var fs = ServiceLocator.fileSystem.GetService();

            var entities = new List<IEntity>
            {
                new PlayerEntity()
                {
                    Name = "Player"
                },
                //new LevelModelEntity(fs.GetAsset("/Maps/gm_fork.bsp"))
                //{
                //    Name = "Generated BSP Mesh"
                //},
                new ModelEntity(fs.GetAsset("/Models/mcrn_tachi/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 0.0125f)
                {
                    Name = "MCRN Tachi"
                },
                //new ModelEntity(fs.GetAsset("/Models/mimicgltf/scene.gltf"), new Vector3d(0, 0, 0), Vector3d.one * 5f)
                //{
                //    Name = "Treasure Chest Mimic"
                //}
            };

            foreach (IEntity entity in entities)
                SceneManager.Instance.AddEntity(entity);

            SetupCustomImGuiMenus();
        }

        private void GenerateTerrain(ref List<IEntity> entities)
        {
            var random = new Random();
            var seed = random.Next(0, 10000);
            var chunksToGenerate = 2;

            for (int x = 0; x < chunksToGenerate; x++)
            {
                for (int z = 0; z < chunksToGenerate; z++)
                {
                    var newEntity = new VoxelChunkEntity(seed, x, z)
                    {
                        Name = $"Voxel Chunk ({x}, {z})",
                    };
                    newEntity.GetComponent<TransformComponent>().Position = new Vector3d(x * 16, 0, z * 16);
                    entities.Add(
                        newEntity
                    );
                }
            }
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
