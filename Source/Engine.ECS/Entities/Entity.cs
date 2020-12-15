using Engine.Assets;
using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Types;
using Engine.Utils.Attributes;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.ECS.Entities
{
    public class Entity<T> : IEntity
    {
        public Guid Id { get; } = Guid.NewGuid();

        public bool Enabled { get; private set; } = true;

        private string name;
        public virtual string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return GetType().Name;

                return name;
            }
            set => name = value;
        }

        public virtual string IconGlyph { get; } = FontAwesome5.Box;

        public virtual IHasParent Parent { get; set; }

        public List<IComponent> Components { get; private set; }

        public Entity()
        {
            Components = new List<IComponent>();
        }

        public virtual void RenderImGui()
        {
            bool enabled = Enabled;
            ImGui.Checkbox("##hidelabel#enabled", ref enabled);
            Enabled = enabled;

            ImGui.SameLine();

            var nameVal = Name;
            ImGui.InputText("##hidelabel#name", ref nameVal, 256);
            Name = nameVal;

            // Entity info
            ImGui.Text($"{IconGlyph} {GetType().Name}");
            ImGui.Text($"({Id})");

            ImGui.Separator();

            // Make a copy to prevent interfering with any changes that occur
            var componentsCopy = new List<IComponent>(Components);

            // Components
            foreach (var component in componentsCopy)
            {
                if (ImGui.TreeNode(component.GetType().Name))
                {
                    component.RenderImGui();
                    ImGui.TreePop();
                }
            }
        }

        /// <summary>
        /// Check for any components with mismatched or missing dependencies.
        /// </summary>
        private void CheckComponentDependencies()
        {
            /* Some components have "require" attributes to ensure that a specific set of components are met (components
             * that depend on other components, e.g. MeshComponent and ShaderComponent).  We should check these for
             * every entity is created in order to ensure that there are no missing components! */

            foreach (var component in Components)
            {
                foreach (var attribute in component.GetType().GetCustomAttributes())
                {
                    if (attribute is RequiresAttribute requiresAttribute)
                    {
                        // Okay - now let's check the RequiresAttribute for any required components:
                        var containsType = false;
                        foreach (var otherComponent in Components)
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

        public virtual void AddComponent(IComponent component)
        {
            if (Components.FindAll(t => { return t.GetType() == component.GetType(); }).Count > 0)
                throw new Exception("Component already exists on this entity.");

            component.Parent = this;
            Components.Add(component);

            // Component added - let's check for any missing component dependencies
            CheckComponentDependencies();
        }

        public virtual T1 GetComponent<T1>()
        {
            var results = Components.FindAll(t => { return t.GetType() == typeof(T1); });
            if (results.Count <= 0)
                throw new Exception("Component doesn't exist on this entity.");

            return (T1)results.First();
        }

        public bool HasComponent<T1>()
        {
            var results = Components.FindAll(t => { return t.GetType() == typeof(T1); });
            return results.Count > 0;
        }

        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            var tmpComponentsCopy = new IComponent[Components.Count];
            Components.CopyTo(tmpComponentsCopy); // Allow component list to be changed mid-notify if necessary

            foreach (var component in tmpComponentsCopy)
            {
                component.OnNotify(notifyType, notifyArgs);
            }
        }

        public void Render()
        {
            foreach (var component in Components)
            {
                component.Render();
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var component in Components)
            {
                component.Update(deltaTime);
            }
        }

        public void RemoveComponent<T1>()
        {
            Components.RemoveAll(t => t.GetType() == typeof(T1));
        }
    }
}
