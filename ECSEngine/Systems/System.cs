using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public class System<T> : ISystem // TODO: Rename System to something else (prevent conflict with .net system)
    {
        public virtual IBase parent { get; set; }
        protected List<IEntity> entities { get; private set; } = new List<IEntity>();

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

        public virtual void Render() { }

        public virtual void Update() { }
    }
}
