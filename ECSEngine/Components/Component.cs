using ECSEngine.Entities;
using ECSEngine.Events;

namespace ECSEngine.Components
{
    /// <summary>
    /// The base class for any component running in the engine.
    /// </summary>
    public class Component<T> : IComponent
    {
        /// <summary>
        /// The component's parent; usually an entity.
        /// </summary>
        public virtual IHasParent parent { get; set; }

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        public virtual void Render() { }

        /// <summary>
        /// Called when an event is triggered.
        /// </summary>
        /// <param name="eventType">The type of the event triggered.</param>
        /// <param name="eventArgs">Any relevant information about the event.</param>
        public virtual void HandleEvent(Event eventType, IEventArgs eventArgs) { }

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        public virtual void Update(float deltaTime) { }

        /// <summary>
        /// Get a component of type T1 from the Component's entity list.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <returns></returns>
        protected virtual T1 GetComponent<T1>()
        {
            return ((IEntity)parent).GetComponent<T1>();
        }
    }
}
