using System.Numerics;

namespace Engine.Utils.MathUtils
{
    internal interface IVector3<T, U>
    {
        T X { get; set; }
        T Y { get; set; }
        T Z { get; set; }

        U Normalized { get; }
        T SqrMagnitude { get; }
        T Magnitude { get; }

        Vector3 ConvertToNumerics();
        void Normalize();
        string ToString();
    }
}
