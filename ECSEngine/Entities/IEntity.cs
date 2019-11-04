using ECSEngine.Events;

namespace ECSEngine
{
    public interface IEntity
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        void Render();

        void Update();

        IComponent GetComponent<T>();
    }
}
