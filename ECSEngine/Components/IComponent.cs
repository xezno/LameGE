﻿using ECSEngine.Events;

namespace ECSEngine
{
    /// <summary>
    /// The base interface for any component running in the engine.
    /// </summary>
    public interface IComponent : IBase
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        void Render();

        void Update();
    }
}
