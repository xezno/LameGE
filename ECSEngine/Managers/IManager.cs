using ECSEngine.Events;

namespace ECSEngine.Managers
{
    public interface IManager : IHasParent
    {
        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="eventArgs">Any relevant information about the event.</param>
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        /// <summary>
        /// Called whenever the manager should run its typical process. (Usually called in game loop).
        /// </summary>
        void Run();
    }
}
