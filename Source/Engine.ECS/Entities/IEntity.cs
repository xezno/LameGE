using Engine.ECS.Components;
using Engine.Events;
using Engine.Types;
using System.Collections.Generic;

namespace Engine.ECS.Entities
{
    public interface IEntity : IHasParent
    {
        string Name { get; set; }
        List<string> Tags { get; set; }

        List<IComponent> Components { get; }
        string IconGlyph { get; }
        bool Enabled { get; set; }

        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="baseEventArgs">Any relevant information about the event.</param>
        void HandleEvent(Event eventType, IEventArgs baseEventArgs);

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        void Render();

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        /// <param name="deltaTime"></param>
        void Update(float deltaTime);

        /// <summary>
        /// Get a component of type T from the Component's entity list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetComponent<T>();

        /// <summary>
        /// Check whether the entity has a specific Component attached.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasComponent<T>();
    }
}
