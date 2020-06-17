using Engine.ECS;
using Engine.Events;

namespace Engine.Utils
{

    // TODO: Is this needed?
    public static class EventController
    {

        public static void BroadcastEvent(Event eventType, IEventArgs eventArgs) // TODO: move to proper event implementation
        {
            foreach (var manager in ManagerRegistry.Instance.Managers)
            {
                manager.HandleEvent(eventType, eventArgs);
            }
        }
    }
}
