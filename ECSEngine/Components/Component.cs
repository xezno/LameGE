using ECSEngine.Attributes;
using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Types;
using ImGuiNET;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quaternion = ECSEngine.MathUtils.Quaternion;

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
        public virtual IHasParent Parent { get; set; }

        /// <summary>
        /// Display draw all available properties within ImGUI.
        /// </summary>
        public virtual void RenderImGui()
        {
            RenderImGuiFields();
        }

        /// <summary>
        /// Get all fields via reflection for use within ImGUI.
        /// </summary>
        /// <param name="depth"></param>
        private void RenderImGuiFields(int depth = 0)
        {
            // TODO: refactor this so it doesnt use dynamic
            if (depth > 1) return; // Prevent any dumb stack overflow errors

            foreach (var field in GetType().GetFields())
            {
                // this works but is, like, the worst solution ever
                var referenceType = typeof(ReflectionRef<>).MakeGenericType(field.FieldType);
                var reference = (dynamic /* alarm bells right here */)Activator.CreateInstance(referenceType, field, this);
                if (field.FieldType == typeof(float))
                {
                    float value = reference.Value;
                    var min = float.MinValue;
                    var max = float.MaxValue;
                    var useSlider = false;
                    var fieldAttributes = field.GetCustomAttributes(false);
                    foreach (var attrib in fieldAttributes.Where(o => o.GetType() == typeof(RangeAttribute)))
                    {
                        var rangeAttrib = (RangeAttribute)attrib;
                        min = rangeAttrib.Min;
                        max = rangeAttrib.Max;
                        useSlider = true;
                    }

                    if (useSlider)
                        ImGui.SliderFloat($"{field.Name}", ref value, min, max);
                    else
                        ImGui.InputFloat($"{field.Name}", ref value);
                    reference.Value = value;
                }
                else if (field.FieldType == typeof(ColorRGB24))
                {
                    var value = new Vector3(reference.Value.r / 255f, reference.Value.g / 255f, reference.Value.b / 255f);
                    ImGui.ColorEdit3($"{field.Name}", ref value);
                    reference.Value = new ColorRGB24((byte)(value.X * 255f), (byte)(value.Y * 255f), (byte)(value.Z * 255f));
                }
                else if (field.FieldType == typeof(MathUtils.Vector3))
                {
                    Vector3 value = reference.Value.ConvertToNumerics();
                    ImGui.DragFloat3(field.Name, ref value, 0.1f);
                    reference.Value = MathUtils.Vector3.ConvertFromNumerics(value);
                }
                else if (field.FieldType == typeof(Quaternion))
                {
                    Vector3 value = reference.Value.ToEulerAngles().ConvertToNumerics();
                    ImGui.DragFloat3(field.Name, ref value, 0.1f);
                    reference.Value = Quaternion.FromEulerAngles(MathUtils.Vector3.ConvertFromNumerics(value));
                }
                else if (field.FieldType == typeof(int))
                {
                    int value = reference.Value;
                    ImGui.DragInt($"{field.Name}", ref value);
                    reference.Value = value;
                }
                else if (field.FieldType == typeof(List<>) || field.FieldType.BaseType == typeof(Array))
                {
                    foreach (var element in (dynamic /* !!! */)field.GetValue(this))
                    {
                        RenderImGuiFields(depth + 1);
                    }
                }
                else
                {
                    ImGui.LabelText($"{field.Name}", $"{field.GetValue(this)}");
                }
            }
        }

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
            return ((IEntity)Parent).GetComponent<T1>();
        }
    }
}
