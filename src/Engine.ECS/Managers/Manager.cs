using Engine.ECS.Entities;
using Engine.Events;
using Engine.Types;
using System;
using System.Collections.Generic;

namespace Engine.ECS.Managers
{
    public class Manager<T> : IManager
    {
        /// <summary>
        /// The manager's parent; usually an instance of Game.
        /// </summary>
        public virtual IHasParent Parent { get; set; }

        public void RenderImGui() { }

        /// <summary>
        /// A list of entities that the manager contains.
        /// </summary>
        public List<IEntity> Entities { get; } = new List<IEntity>();

        // All systems should be singletons.
        private static T privateInstance;

        /// <summary>
        /// Get the single instance of the desired manager.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (privateInstance == null)
                    CreateInstance();
                return privateInstance;
            }
        }

        public static void CreateInstance()
        {
            if (privateInstance == null)
                privateInstance = Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="eventArgs">Any relevant information about the event.</param>
        public virtual void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            foreach (var entity in Entities)
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
            entity.Parent = this;
            Entities.Add(entity);
        }
    }
}
