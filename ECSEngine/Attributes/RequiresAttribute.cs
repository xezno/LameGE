using System;

namespace ECSEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class RequiresAttribute : Attribute
    {
        public Type requiredType;

        public RequiresAttribute(Type requiredType)
        {
            this.requiredType = requiredType;
        }
    }
}
