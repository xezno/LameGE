using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public class System<T> : ISystem
    {
        public List<IEntity> entities { get; set; }

        public virtual void BroadcastEvent(Event eventType, IEventArgs eventArgs)
        {
            foreach (IEntity entity in entities)
            {
                entity.HandleEvent(eventType, eventArgs);
            }
        }
    }
}
