using ECSEngine.Events;
using ECSEngine.Systems;

namespace ECSEngine
{
    public static class EventManager
    {
        static WorldSystem worldSystem;

        public static void RegisterWorldSystem(WorldSystem newWorldSystem) // TODO: find a better way of doing this (i.e. singletons)
        {
            worldSystem = newWorldSystem;
        }

        public static void BroadcastEvent(Event eventType, IEventArgs eventArgs) // TODO: move to proper event implementation
        {
            worldSystem.BroadcastEvent(eventType, eventArgs);
        }
    }
}
