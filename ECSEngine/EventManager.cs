using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ECSEngine.Events;
using ECSEngine.Systems;

namespace ECSEngine
{
    // TODO: Is this needed?
    public static class EventManager
    {
        private static List<ISystem> systems = new List<ISystem>();

        public static void AddSystem(ISystem system)
        {
            systems.Add(system);
        }

        public static void BroadcastEvent(Event eventType, IEventArgs eventArgs) // TODO: move to proper event implementation
        {
            foreach (ISystem system in systems)
            {
                system.HandleEvent(eventType, eventArgs);
            }
        }
    }
}
