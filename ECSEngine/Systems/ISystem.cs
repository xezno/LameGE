using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public interface ISystem : IBase
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
        void Update();
    }
}
