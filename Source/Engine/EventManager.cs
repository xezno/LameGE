using Engine.ECS.Managers;
using Engine.Events;
using System.Collections.Generic;

namespace Engine
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
            foreach (var system in systems)
            {
                system.HandleEvent(eventType, eventArgs);
            }
        }
    }
}
