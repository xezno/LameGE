using System.Collections.Generic;

using ECSEngine.Entities;

namespace ECSEngine.Systems
{
    public class WorldSystem : System<WorldSystem>
    {
        public CameraEntity mainCamera;

        public WorldSystem()
        {
            this.entities = new List<IEntity>();
            mainCamera = new CameraEntity();
            AddEntity(mainCamera);
        }

        public void Render()
        {
            foreach (IEntity entity in entities)
            {
                entity.Render();
            }
        }

        public void Update()
        {
            foreach (IEntity entity in entities)
            {
                entity.Update();
            }
        }
    }
}
