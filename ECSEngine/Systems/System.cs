using ECSEngine.Events;
using System.Collections.Generic;

namespace ECSEngine.Systems
{
    public class System<T> : ISystem
    {
        public virtual IBase parent { get; set; }
        public List<IEntity> entities { get; set; }

        public virtual void BroadcastEvent(Event eventType, IEventArgs eventArgs)
        {
            foreach (IEntity entity in entities)
            {
                entity.HandleEvent(eventType, eventArgs);
            }
        }

        public virtual void AddEntity(IEntity entity)
        {
            entity.parent = this;
            entities.Add(entity);
        }
    }
}
