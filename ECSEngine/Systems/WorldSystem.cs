using System.Collections.Generic;

using ECSEngine.Entities;

namespace ECSEngine.Systems
{
    public class WorldSystem : System<WorldSystem>
    {
        public CameraEntity mainCamera;

        public WorldSystem(params IEntity[] entities)
        {
            mainCamera = new CameraEntity();
            AddEntity(mainCamera);

            foreach (IEntity entity in entities)
                AddEntity(entity);
        }

        public override void Render()
        {
            foreach (IEntity entity in entities)
            {
                entity.Render();
            }
        }

        public override void Update()
        {
            foreach (IEntity entity in entities)
            {
                entity.Update();
            }
        }
    }
}