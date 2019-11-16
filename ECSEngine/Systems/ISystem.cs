using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public interface ISystem : IBase
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);
        void Render();
        void Update();
    }
}
