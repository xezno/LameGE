using ECSEngine.Events;

namespace ECSEngine.Components
{
    /// <summary>
    /// The base class for any component running in the engine.
    /// </summary>
    public class Component<T> : IComponent
    {
        public virtual IBase parent { get; set; }

        public virtual void Render() { }

        public virtual void HandleEvent(Event eventType, IEventArgs eventArgs) { }

        public virtual void Update() { }

        protected virtual T1 GetComponent<T1>()
        {
            return ((IEntity)parent).GetComponent<T1>();
        }
    }
}
