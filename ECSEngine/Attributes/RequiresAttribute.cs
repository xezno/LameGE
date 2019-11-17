using System;

namespace ECSEngine.Attributes
{
    /// <summary>
    /// This attribute will inform the engine that one component relies on another component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class RequiresAttribute : Attribute
    {
        /// <summary>
        /// The type of the component that this component relies on.
        /// </summary>
        public Type requiredType;

        /// <summary>
        /// Construct the Requires attribute.
        /// </summary>
        /// <param name="requiredType">The type of the component that this component relies on.</param>
        public RequiresAttribute(Type requiredType)
        {
            this.requiredType = requiredType;
        }
    }
}
