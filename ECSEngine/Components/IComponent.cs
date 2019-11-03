using ECSEngine.Events;

namespace ECSEngine
{
    public interface IComponent
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        void Draw();

        void Update();
    }
}
