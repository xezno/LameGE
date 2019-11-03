using ECSEngine.Events;

namespace ECSEngine
{
    /// <summary>
    /// The base interface for any component running in the engine.
    /// </summary>
    public interface IComponent
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        void Draw();

        void Update();
    }
}
