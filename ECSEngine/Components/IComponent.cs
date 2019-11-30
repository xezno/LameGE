﻿using ECSEngine.Events;

namespace ECSEngine.Components
{
    /// <summary>
    /// The base interface for any component running in the engine.
    /// </summary>
    public interface IComponent : IHasParent
    {
        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="eventArgs">Any relevant information about the event.</param>
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        void Render();

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        void Update(float deltaTime);
    }
}
