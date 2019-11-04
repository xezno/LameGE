using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine.Systems
{
    public interface ISystem
    {
        List<IEntity> entities { get; set; }
        void BroadcastEvent(Event eventType, IEventArgs eventArgs);
    }
}
