using System.Collections.Generic;
using ECSEngine.Entities;

namespace ECSEngine.Systems
{
    public sealed class WorldSystem : System<WorldSystem>
    {
        /// <summary>
        /// The main world camera used to render all entities.
        /// </summary>
        public readonly CameraEntity mainCamera;

        public List<LightEntity> lights;

        /// <summary>
        /// Construct a world system containing any entities required.
        /// </summary>
        public WorldSystem()
        {
            mainCamera = new CameraEntity();
            AddEntity(mainCamera);

            lights = new List<LightEntity>();
            var mainLight = new LightEntity();
            lights.Add(mainLight);
            AddEntity(mainLight);
        }

        /// <summary>
        /// Render all the entities within the world system.
        /// </summary>
        public override void Render()
        {
            foreach (IEntity entity in entities)
            {
                entity.Render();
            }
        }

        /// <summary>
        /// Update all the entities within the world system.
        /// </summary>
        public override void Update()
        {
            foreach (IEntity entity in entities)
            {
                entity.Update();
            }
        }
    }
}