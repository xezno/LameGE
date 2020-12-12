using Engine;
using Engine.ECS.Entities;
using System.Collections.Generic;
using Example.Entities;
using Engine.Common.MathUtils;
using Engine.Common;
using Quincy.Components;
using System;
using Engine.ECS.Observer;
using Example.States;
using Engine.FSM.Managers;

namespace Example
{
    internal sealed class ExampleGame : Game
    {
        public ExampleGame(string gamePropertyPath) : base(gamePropertyPath) { }

        protected override void InitScene()
        {
            base.InitScene();
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

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            base.OnNotify(eventType, notifyArgs);
            switch (eventType)
            {
                case NotifyType.LoadFinished:
                    StateManager.Instance.ChangeState(new ExampleGameState());
                    break;
            }
        }
    }
}
