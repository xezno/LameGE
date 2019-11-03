using ECSEngine.Events;
using ECSEngine.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
