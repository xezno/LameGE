using ECSEngine.Events;
using ECSEngine.Systems;

namespace ECSEngine
{
    public static class EventManager
    {
        static WorldSystem worldSystem;

        public static void RegisterWorldSystem(WorldSystem worldSystem_) // TODO: find a better way of doing this (i.e. singletons)
        {
            worldSystem = worldSystem_;
        }

        public static void BroadcastEvent(Event eventType, IEventArgs eventArgs)
        {
            worldSystem.BroadcastEvent(eventType, eventArgs);
        }
    }
}
