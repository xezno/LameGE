using ECSEngine.Events;

namespace ECSEngine.Components
{
    // TODO: Consider - is this necessary? Might be worth just using interfaces instead
    // of building a "base" class on top of the interface; less complicated that way.
    public abstract class Component<T> : IComponent
    {
        public abstract void Draw();

        public abstract void HandleEvent(Event eventType, IEventArgs eventArgs);

        public abstract void Update();
    }
}
