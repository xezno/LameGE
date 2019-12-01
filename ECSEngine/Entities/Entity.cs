using ECSEngine.Attributes;
using ECSEngine.Components;
using ECSEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ECSEngine.Entities
{
    public class Entity<T> : IEntity
    {
        /// <summary>
        /// The entity's parent; usually a manager.
        /// </summary>
        public virtual IHasParent parent { get; set; }

        /// <summary>
        /// A list of components that the entity contains.
        /// </summary>
        public List<IComponent> components { get; private set; }

        /// <summary>
        /// Construct an <see cref="Entity{T}"/>.
        /// </summary>
        public Entity()
        {
            components = new List<IComponent>();
        }

        /// <summary>
        /// Check for any components with mismatched or missing dependencies.
        /// </summary>
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
                        foreach (var otherComponent in components)
                        {
                            if (otherComponent.GetType() == requiresAttribute.requiredType)
                            {
                                containsType = true;
                                break;
                            }
                        }
                        if (!containsType) throw new Exception($"Dependency requirements for {component.GetType().Name} are not fully met.");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Add a component to an entity. This will also check for component dependencies.
        /// </summary>
        /// <param name="component">An instance of the desired component to add.</param>
        public virtual void AddComponent(IComponent component) // TODO: Consider just using a property instead of this cos its 10x cooler
        {
            component.parent = this;
            components.Add(component);

            // Component added - let's check for any missing component dependencies
            CheckComponentDependencies();
        }

        /// <summary>
        /// Retrieve a component of type <typeparamref name="T1"/> from an entity.
        /// </summary>
        /// <typeparam name="T1">The desired type of component.</typeparam>
        /// <returns>The first component of type <typeparamref name="T1"/> from the entity's component list.</returns>
        public virtual T1 GetComponent<T1>() => (T1)(components.FindAll((t) => { return t.GetType() == typeof(T1); }).First());

        public virtual void HandleEvent(Event eventType, IEventArgs baseEventArgs) { }

        public virtual void Render()
        {
            foreach (IComponent component in components)
            {
                component.Render();
            }
        }

        public virtual void Update(float deltaTime)
        {
            foreach (IComponent component in components)
            {
                component.Update(deltaTime);
            }
        }
    }
}
