using Engine.ECS.Notify;
using Engine.ECS.Entities;
using Engine.Types;
using Engine.Utils.Attributes;
using ImGuiNET;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Quaternion = Engine.Utils.MathUtils.Quaternion;

namespace Engine.ECS.Components
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
            RenderImGuiMembers();
        }

        /// <summary>
        /// Get all fields via reflection for use within ImGUI.
        /// </summary>
        /// <param name="depth">The depth of the current reflection.</param>
        private void RenderImGuiMembers(int depth = 0)
        {
            if (depth > 1) return; // Prevent any dumb stack overflow errors

            foreach (var field in GetType().GetFields())
            {
                RenderImGuiMember(field, ref depth);
            }
            foreach (var property in GetType().GetProperties())
            {
                RenderImGuiMember(property, ref depth);
            }
        }

        /// <summary>
        /// Handles the rendering of a Type's member through ImGui, allowing for prototyping.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="depth"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void RenderImGuiMember(MemberInfo memberInfo, ref int depth)
        {
            // TODO: refactor this so it doesnt use dynamic
            Type type;
            dynamic memberValue;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    memberValue = ((FieldInfo)memberInfo).GetValue(this);
                    type = ((FieldInfo)memberInfo).FieldType;
                    break;
                case MemberTypes.Property:
                    memberValue = ((PropertyInfo)memberInfo).GetValue(this);
                    type = ((PropertyInfo)memberInfo).PropertyType;
                    break;
                default:
                    throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
            }

            // this works but is, like, the worst solution ever
            var referenceType = typeof(ReflectionRef<>).MakeGenericType(type);
            var reference = (dynamic /* alarm bells right here */)Activator.CreateInstance(referenceType, memberInfo, this);
            if (type == typeof(float))
            {
                DrawImGuiFloat(memberInfo, reference);
            }
            else if (type == typeof(ColorRGB24))
            {
                DrawImGuiColor(memberInfo, reference);
            }
            else if (type == typeof(Utils.MathUtils.Vector3))
            {
                DrawImGuiVector3(memberInfo, reference);
            }
            else if (type == typeof(Vertex3f))
            {
                DrawImguiVertex3f(memberInfo, reference);
            }
            else if (type == typeof(Quaternion))
            {
                DrawImGuiQuaternion(memberInfo, reference);
            }
            else if (type == typeof(int))
            {
                DrawImGuiInt(memberInfo, reference);
            }
            else if (type == typeof(List<>) || type.BaseType == typeof(Array))
            {
                DrawImGuiArray(memberValue, depth);
            }
            else
            {
                ImGui.LabelText($"{memberInfo.Name}", $"{memberValue}");
            }
        }

        private void DrawImguiVertex3f(MemberInfo field, dynamic reference)
        {
            Vector3 value = new Vector3(reference.Value.x, reference.Value.y, reference.Value.z);
            ImGui.DragFloat3(field.Name, ref value, 0.1f);
            reference.Value = new Vertex3f(value.X, value.Y, value.Z);
        }

        private void DrawImGuiFloat(MemberInfo field, dynamic reference)
        {
            float value = reference.Value;
            var min = float.MinValue;
            var max = float.MaxValue;
            var useSlider = false;
            var fieldAttributes = field.GetCustomAttributes(false);
            foreach (var attrib in fieldAttributes.Where(o => o is RangeAttribute))
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

        private void DrawImGuiColor(MemberInfo field, dynamic reference)
        {
            var value = new Vector3(reference.Value.r / 255f, reference.Value.g / 255f, reference.Value.b / 255f);
            ImGui.ColorEdit3($"{field.Name}", ref value);
            reference.Value = new ColorRGB24((byte)(value.X * 255f), (byte)(value.Y * 255f), (byte)(value.Z * 255f));
        }

        private void DrawImGuiVector3(MemberInfo field, dynamic reference)
        {
            Vector3 value = reference.Value.ConvertToNumerics();
            ImGui.DragFloat3(field.Name, ref value, 0.1f);
            reference.Value = Utils.MathUtils.Vector3.ConvertFromNumerics(value);
        }

        private void DrawImGuiQuaternion(MemberInfo field, dynamic reference)
        {
            Vector3 value = reference.Value.ToEulerAngles().ConvertToNumerics();
            ImGui.DragFloat3(field.Name, ref value, 0.1f);
            reference.Value = Quaternion.FromEulerAngles(Utils.MathUtils.Vector3.ConvertFromNumerics(value));
        }

        private void DrawImGuiInt(MemberInfo field, dynamic reference)
        {
            int value = reference.Value;
            ImGui.DragInt($"{field.Name}", ref value);
            reference.Value = value;
        }

        private void DrawImGuiArray(dynamic memberValue, int depth)
        {
            foreach (var element in memberValue)
            {
                RenderImGuiMembers(depth + 1);
            }
        }

        private ImGuiInputTextFlags GetFlags(dynamic reference)
        {
            return reference.CanGet ? ImGuiInputTextFlags.None : ImGuiInputTextFlags.ReadOnly;
        }

        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        public virtual void Render() { }

        /// <summary>
        /// Called when an notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs) { }

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
