using System;

namespace Engine.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RangeAttribute : Attribute
    {
        public float Min { get; } // inclusive
        public float Max { get; } // inclusive
        public RangeAttribute(float min = 0.0f, float max = 1.0f)
        {
            Min = min;
            Max = max;
        }
    }
}
