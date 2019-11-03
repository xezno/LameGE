using System;
using System.Collections.Generic;
using System.Linq;

using ECSEngine.Components;
using ECSEngine.Events;

namespace ECSEngine.Entities
{
    public class TestModelEntity : IEntity
    {
        public List<IComponent> components { get; set; }

        public TestModelEntity()
        {
            this.components = new List<IComponent>(); /* TODO: Consider - since this should also (see: IEntity) be 
            instantiated, also consider moving this elsewhere. (maybe we need those base classes over the 
            interfaces after all?) */

            // Add mesh component
            this.components.Add(new MeshComponent("Content/TestMesh.obj"));
        }

        public IComponent GetComponent<T>()
        {
            var matchingComponents = this.components.Where((t) => t.GetType() == typeof(T));
            if (matchingComponents.Count() > 0) return matchingComponents.First();

            // No match, so throw an exception bigger than the sun because the person who invoked this was an idiot
            throw new Exception($"Component of type {typeof(T).Name} does not exist on {this.GetType().Name}");
        }

        public void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            Debug.Log($"Received event {eventType.ToString()}");
        }
    }
}
