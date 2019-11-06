using ECSEngine.Events;

namespace ECSEngine
{
    public interface IEntity : IBase
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        void Render();

        void Update();

        T GetComponent<T>();
    }
}
