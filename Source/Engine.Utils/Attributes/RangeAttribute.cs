using System;

namespace Engine.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RangeAttribute : Attribute
    {
        /// <summary>
        /// An inclusive minimum for the value.
        /// </summary>
        public float Min { get; } // inclusive
        /// <summary>
        /// An inclusive maximum for the value.
        /// </summary>
        public float Max { get; } // inclusive

        /// <summary>
        /// Sets a range for this field or property [min <= x <= max]
        /// </summary>
        /// <param name="min">An inclusive minimum for the value.</param>
        /// <param name="max">An inclusive maximum for the value.</param>
        public RangeAttribute(float min = 0.0f, float max = 1.0f)
        {
            Min = min;
            Max = max;
        }
    }
}
