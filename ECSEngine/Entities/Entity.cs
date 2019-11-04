using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ECSEngine.Attributes;
using ECSEngine.Events;

namespace ECSEngine.Entities
{
    public class Entity<T> : IEntity
    {
        List<IComponent> components { get; set; }

        public Entity()
        {
            components = new List<IComponent>();
        }

        private void CheckComponentDependencies()
        {
            /* Some components have "require" attributes to ensure that a specific set of components are met (components
             * that depend on other components, e.g. MeshComponent and ShaderComponent).  We should check these for
             * every entity is created in order to ensure that there are no missing components! */

            foreach (var component in components)
            {
                foreach (var attribute in component.GetType().GetCustomAttributes())
                {
                    if (attribute is RequiresAttribute)
                    {
                        // Okay - now let's check the RequiresAttribute for any required components:
                        RequiresAttribute requiresAttribute = (RequiresAttribute)attribute;
                        bool containsType = false;
                        foreach (var component_ in components)
                        {
                            if (component_.GetType() == requiresAttribute.requiredType)
                            {
                                containsType = true;
                                break;
                            }
                        }
                        if (!containsType) throw new Exception($"Dependancy requirements for {component.GetType().Name} are not fully met.");
                        break;
                    }
                }
            }
        }

        public virtual void AddComponent(IComponent component)
        {
            components.Add(component);

            // Component added - let's check for any missing component dependencies
            CheckComponentDependencies();
        }

        public virtual IComponent GetComponent<T1>() => components.FindAll((t) => { return t.GetType() == typeof(T1); }).First();

        public virtual void HandleEvent(Event eventType, IEventArgs eventArgs) { }

        public virtual void Render()
        {
            foreach (IComponent component in components)
            {
                component.Render();
            }
        }

        public virtual void Update() 
        {
            foreach (IComponent component in components)
            {
                component.Update();
            }
        }
    }
}
