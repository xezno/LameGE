using System.Collections.Generic;

namespace ECSEngine.Systems
{
    public class WorldSystem : ISystem
    {
        public List<IEntity> entities;
        public WorldSystem()
        {
            entities = new List<IEntity>();
        }
    }
}
