using System;
using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public class WorldSystem : System<WorldSystem>
    {
        private List<IEntity> entities;

        public WorldSystem(List<IEntity> entities)
        {
            this.entities = entities;
        }

        public void BroadcastEvent(Event eventType, IEventArgs eventArgs)
        {
            foreach (IEntity entity in entities)
            {
                entity.HandleEvent(eventType, eventArgs);
            }
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
