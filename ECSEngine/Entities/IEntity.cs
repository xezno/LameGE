using System.Collections.Generic;

using ECSEngine.Events;

namespace ECSEngine
{
    public interface IEntity
    {
        List<IComponent> components { get; set; }
        void HandleEvent(Event eventType, IEventArgs eventArgs);

        IComponent GetComponent<T>(); /* TODO: Consider - this is the same for every entity - it's 
        just components.Where((t)=>return t.GetType() == typeof(T)) or something like that - so it 
        might be better practice to somehow make this virtual or move it elsewhere? Leaving it here 
        for now, just so that it's easier to test, but NEEDS looking at asap. */
    }
}
