using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public class WorldSystem : System<WorldSystem>
    {
        public WorldSystem(List<IEntity> entities)
        {
            this.entities = entities;
        }

        public void Render()
        {
            foreach (IEntity entity in entities)
            {
                entity.Render();
            }
        }
    }
}
