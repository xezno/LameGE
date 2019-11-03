using System.Collections.Generic;

namespace ECSEngine.Systems
{
    public class WorldSystem : System<WorldSystem>
    {
        public List<IEntity> entities;
        public WorldSystem()
        {
            entities = new List<IEntity>();
        }
    }
}
