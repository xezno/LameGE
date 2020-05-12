using System;
using System.Reflection;

namespace Engine.Types
{
    public class ReflectionRef<T>
    {
        private readonly MemberInfo memberInfo;
        private readonly object origin;

        public T Value
        {
            get
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        return (T)((FieldInfo)memberInfo).GetValue(origin);
                    case MemberTypes.Property: // TODO: Check if there is no get method & hide in imgui?
                        var propertyInfo = ((PropertyInfo)memberInfo);
                        if (propertyInfo.GetMethod != null)
                            return (T)propertyInfo.GetValue(origin);
                        else
                            throw new NotImplementedException($"Property has no get method");
                }
                throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
            }
            set
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        ((FieldInfo)memberInfo).SetValue(origin, value);
                        break;
                    case MemberTypes.Property: // TODO: Check if there is no set method beforehand, set as readonly within imgui
                        var propertyInfo = ((PropertyInfo)memberInfo);
                        if (propertyInfo.SetMethod != null)
                            propertyInfo.SetValue(origin, value);
                        break;
                    default:
                        throw new NotImplementedException($"Member type {memberInfo.MemberType} not implemented");
                }
            }
        }

        public ReflectionRef(MemberInfo memberInfo, object origin)
        {
            this.memberInfo = memberInfo;
            this.origin = origin;
        }
    }
}
