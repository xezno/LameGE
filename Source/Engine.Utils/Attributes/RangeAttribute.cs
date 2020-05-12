using System;

namespace Engine.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RangeAttribute : Attribute
    {
        public float Min { get; }
        public float Max { get; }
        public RangeAttribute(float min = 0.0f, float max = 1.0f)
        {
            Min = min;
            Max = max;
        }
    }
}
