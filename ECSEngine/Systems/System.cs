using System;
using System.Collections.Generic;

using ECSEngine.Entities;
using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public class System<T> : ISystem // TODO: Rename System to something else (prevent conflict with .net system)
    {
        /// <summary>
        /// The system's parent; usually an instance of Game.
        /// </summary>
        public virtual IBase parent { get; set; }

        /// <summary>
        /// A list of entities that the system contains.
        /// </summary>
        protected List<IEntity> entities { get; } = new List<IEntity>();

        // All systems should be singletons.
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null) _instance = Activator.CreateInstance<T>();
                return _instance;
            }
        }

        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="eventArgs">Any relevant information about the event.</param>
        public virtual void HandleEvent(Event eventType, IEventArgs eventArgs)
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

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        public virtual void Render() { }


        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        public virtual void Update(float deltaTime) { }
    }
}
