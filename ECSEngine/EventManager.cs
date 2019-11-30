using System.Collections.Generic;

using ECSEngine.Events;
using ECSEngine.Managers;

namespace ECSEngine
{
    // TODO: Is this needed?
    public static class EventManager
    {
        private static List<IManager> systems = new List<IManager>();

        public static void AddManager(IManager manager) // Managers should not have sub-managers!
        {
            systems.Add(manager);
        }

        public static void BroadcastEvent(Event eventType, IEventArgs eventArgs) // TODO: move to proper event implementation
        {
            foreach (IManager system in systems)
            {
                system.HandleEvent(eventType, eventArgs);
            }
        }
    }
}
