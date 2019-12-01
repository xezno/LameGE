using ECSEngine.Entities;
using ECSEngine.Events;
using System;
using System.Collections.Generic;

namespace ECSEngine.Managers
{
    public class Manager<T> : IManager
    {
        /// <summary>
        /// The manager's parent; usually an instance of Game.
        /// </summary>
        public virtual IHasParent parent { get; set; }

        /// <summary>
        /// A list of entities that the manager contains.
        /// </summary>
        public List<IEntity> entities { get; } = new List<IEntity>();

        // All systems should be singletons.
        private static T privateInstance;

        /// <summary>
        /// Get the single instance of the desired manager.
        /// </summary>
        public static T instance
        {
            get
            {
                if (privateInstance == null) privateInstance = Activator.CreateInstance<T>();
                return privateInstance;
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

        /// <summary>
        /// Called whenever the manager should run its typical process. (Usually called in game loop).
        /// </summary>
        public virtual void Run() { }

        public void AddEntity(IEntity entity)
        {
            entity.parent = this;
            entities.Add(entity);
        }
    }
}
