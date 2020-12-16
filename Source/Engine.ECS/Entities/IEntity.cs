using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Types;
using System;
using System.Collections.Generic;

namespace Engine.ECS.Entities
{
    public interface IEntity : IHasParent, IObserver
    {
        /// <summary>
        /// A human-friendly identifier for this entity.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A list of components that are owned by this entity.
        /// </summary>
        List<IComponent> Components { get; }

        /// <summary>
        /// A character or character pair, used within the ImGui editor to represent this entity.
        /// </summary>
        string IconGlyph { get; }

        /// <summary>
        /// Determines whether or not the entity is updated or rendered.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// A globally unique identifier for this entity.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Called when an notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs);

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

        /// <summary>
        /// Add a component to an entity. This will also check for component dependencies.
        /// Only one component of a given type can be attached to an entity at one time.
        /// </summary>
        /// <param name="component">An instance of the desired component to add.</param>
        void AddComponent(IComponent component);

        /// <summary>
        /// Remove a component from an entity.
        /// </summary>
        /// <typeparam name="T">An instance of the desired component to add.</typeparam>
        void RemoveComponent<T>();
    }
}
