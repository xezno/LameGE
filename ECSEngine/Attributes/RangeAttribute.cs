using System;

namespace ECSEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    class RangeAttribute : Attribute
    {
        public float min { get; }
        public float max { get; }
        public RangeAttribute(float min = 0.0f, float max = 1.0f)
        {
            this.min = min;
            this.max = max;
        }
    }
}
