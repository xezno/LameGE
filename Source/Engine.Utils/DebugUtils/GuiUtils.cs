using Engine.Types;
using Engine.Common.Attributes;
using ImGuiNET;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Engine.Common.DebugUtils
{
    public static class GuiUtils
    {
        public static void DrawImGuiVertex3f(string name, object reference)
        {
            var tmpReference = reference as ReflectionRef<Vertex3f>;
            Vector3 value = new Vector3(tmpReference.Value.x, tmpReference.Value.y, tmpReference.Value.z);
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat3($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            (reference as ReflectionRef<Vertex3f>).Value = new Vertex3f(value.X, value.Y, value.Z);
        }

        public static void DrawImGuiVertex2f(string name, object reference)
        {
            var tmpReference = reference as ReflectionRef<Vertex2f>;
            Vector2 value = new Vector2(tmpReference.Value.x, tmpReference.Value.y);
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat2($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            (reference as ReflectionRef<Vertex2f>).Value = new Vertex2f(value.X, value.Y);
        }

        public static void DrawImGuiFloat(string name, object reference, float minRange = float.MinValue, float maxRange = float.MaxValue)
        {
            float value = (reference as ReflectionRef<float>).Value;
            var min = float.MinValue;
            var max = float.MaxValue;
            var useSlider = (minRange > float.MaxValue) && (maxRange < float.MaxValue);

            ImGui.Text(name);
            ImGui.NextColumn();

            if (useSlider)
                ImGui.SliderFloat($"{name}##hidelabel", ref value, min, max);
            else
                ImGui.InputFloat($"{name}##hidelabel", ref value);
            ImGui.NextColumn();
            (reference as ReflectionRef<float>).Value = value;
        }

        public static void DrawImGuiColor(string name, object reference)
        {
            var tmpReference = reference as ReflectionRef<ColorRGB24>;
            var value = new Vector3(tmpReference.Value.r / 255f, tmpReference.Value.g / 255f, tmpReference.Value.b / 255f);
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.ColorEdit3($"{name}##hidelabel", ref value);
            ImGui.NextColumn();
            (reference as ReflectionRef<ColorRGB24>).Value = new ColorRGB24((byte)(value.X * 255f), (byte)(value.Y * 255f), (byte)(value.Z * 255f));
        }

        public static void DrawImGuiVector3(string name, object reference)
        {
            Vector3 value = (reference as ReflectionRef<MathUtils.Vector3f>).Value.ConvertToNumerics();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat3($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            (reference as ReflectionRef<MathUtils.Vector3f>).Value = MathUtils.Vector3f.ConvertFromNumerics(value);
        }

        public static void DrawImGuiVector3d(string name, object reference)
        {
            Vector3 value = (reference as ReflectionRef<MathUtils.Vector3d>).Value.ConvertToNumerics();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat3($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            (reference as ReflectionRef<MathUtils.Vector3d>).Value = MathUtils.Vector3d.ConvertFromNumerics(value);
        }

        public static void DrawImGuiVector2(string name, object reference)
        {
            Vector2 value = (reference as ReflectionRef<MathUtils.Vector2f>).Value.ConvertToNumerics();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat2($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            if (value != (reference as ReflectionRef<MathUtils.Vector2f>).Value.ConvertToNumerics())
                (reference as ReflectionRef<MathUtils.Vector2f>).Value = MathUtils.Vector2f.ConvertFromNumerics(value);
        }

        public static void DrawImGuiQuaternion(string name, object reference)
        {
            Vector3 value = (reference as ReflectionRef<MathUtils.Quaternion>).Value.ToEulerAngles().ConvertToNumerics();
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragFloat3($"{name}##hidelabel", ref value, 0.1f);
            ImGui.NextColumn();
            (reference as ReflectionRef<MathUtils.Quaternion>).Value = MathUtils.Quaternion.FromEulerAngles(MathUtils.Vector3f.ConvertFromNumerics(value));
        }

        public static void DrawImGuiInt(string name, object reference)
        {
            int value = (reference as ReflectionRef<int>).Value;
            ImGui.Text(name);
            ImGui.NextColumn();
            ImGui.DragInt($"{name}##hidelabel", ref value);
            ImGui.NextColumn();
            (reference as ReflectionRef<int>).Value = value;
        }

        public static void DrawImGuiArray(dynamic memberValue, int depth)
        {
            var depth_ = ++depth;
            foreach (var value in memberValue)
            {
                RenderImGuiMembers(value, depth_);
            }
        }

        public static void DrawFilePath(string name, object reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the rendering of a Type's member through ImGui, allowing for prototyping.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="depth"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void RenderImGuiMember(object obj, MemberInfo memberInfo, ref int depth)
        {
            Type type;
            dynamic memberValue;
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    memberValue = ((FieldInfo)memberInfo).GetValue(obj);
                    type = ((FieldInfo)memberInfo).FieldType;
                    break;
                case MemberTypes.Property:
                    memberValue = ((PropertyInfo)memberInfo).GetValue(obj);
                    type = ((PropertyInfo)memberInfo).PropertyType;
                    break;
                default:
                    throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
            }

            var referenceType = typeof(ReflectionRef<>).MakeGenericType(type);
            var reference = Activator.CreateInstance(referenceType, memberInfo, obj);

            if (Attribute.IsDefined(memberInfo, typeof(HideInImGuiAttribute)))
            {
                return;
            }

            if (type == typeof(float))
            {
                var minValue = float.MinValue;
                var maxValue = float.MaxValue;
                var fieldAttributes = memberInfo.GetCustomAttributes(false);
                foreach (var attrib in fieldAttributes.Where(o => o is RangeAttribute))
                {
                    var rangeAttrib = (RangeAttribute)attrib;
                    minValue = rangeAttrib.Min;
                    maxValue = rangeAttrib.Max;
                }
                DrawImGuiFloat(memberInfo.Name, reference, minValue, maxValue);
            }
            else if (type == typeof(ColorRGB24))
            {
                DrawImGuiColor(memberInfo.Name, reference);
            }
            else if (type == typeof(MathUtils.Vector2f))
            {
                DrawImGuiVector2(memberInfo.Name, reference);
            }
            else if (type == typeof(Vertex2f))
            {
                DrawImGuiVertex2f(memberInfo.Name, reference);
            }
            else if (type == typeof(MathUtils.Vector3f))
            {
                DrawImGuiVector3(memberInfo.Name, reference);
            }
            else if (type == typeof(MathUtils.Vector3d))
            {
                DrawImGuiVector3d(memberInfo.Name, reference);
            }
            else if (type == typeof(Vertex3f))
            {
                DrawImGuiVertex3f(memberInfo.Name, reference);
            }
            else if (type == typeof(MathUtils.Quaternion))
            {
                DrawImGuiQuaternion(memberInfo.Name, reference);
            }
            else if (type == typeof(int))
            {
                DrawImGuiInt(memberInfo.Name, reference);
            }
            else if (type == typeof(List<>) || type.BaseType == typeof(Array))
            {
                DrawImGuiArray(memberValue, depth);
            }
            else
            {
                ImGui.Text(memberInfo.Name);
                ImGui.NextColumn();
                ImGui.Text($"{memberValue}");
                ImGui.NextColumn();
            }
        }

        /// <summary>
        /// Get all fields via reflection for use within ImGUI.
        /// </summary>
        /// <param name="depth">The depth of the current reflection.</param>
        public static void RenderImGuiMembers(object obj, int depth = 0)
        {
            ImGui.Columns(2);
            if (depth > 1)
            {
                ImGui.Columns(1);
                return; // Prevent any dumb stack overflow errors
            }

            foreach (var field in obj.GetType().GetFields())
            {
                RenderImGuiMember(obj, field, ref depth);
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                RenderImGuiMember(obj, property, ref depth);
            }

            ImGui.Columns(1);
        }
    }
}
