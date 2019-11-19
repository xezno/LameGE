using System.Collections.Generic;
using ECSEngine.Entities;

namespace ECSEngine.Systems
{
    public sealed class WorldSystem : System<WorldSystem>
    {
        /// <summary>
        /// The main world camera used to render all entities.
        /// </summary>
        public CameraEntity mainCamera;

        public List<LightEntity> lights;

        /// <summary>
        /// Construct a world system containing any entities required.
        /// </summary>
        /// <param name="entities">Any number of entities to push to the world system's list of entities.</param>
        public WorldSystem(params IEntity[] entities)
        {
            mainCamera = new CameraEntity();
            lights = new List<LightEntity>();

            AddEntity(mainCamera);

            lights.Add(new LightEntity());

            foreach (IEntity entity in entities)
            {
                AddEntity(entity);
            }
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