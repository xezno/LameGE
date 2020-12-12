using Engine.Common.MathUtils;
using System.Runtime.InteropServices;

namespace Quincy
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vertex
    {
        public Vector3f Position { get; set; }
        public Vector3f Normal { get; set; }
        public Vector2f TexCoords { get; set; }
        public Vector3f Tangent { get; set; }
        public Vector3f BiTangent { get; set; }

        public static bool operator== (Vertex a, Vertex b)
        {
            return (a.Position == b.Position) && (a.Normal == b.Normal) && (a.TexCoords == b.TexCoords) && (a.Tangent == b.Tangent) && (a.BiTangent == b.BiTangent);
        }

        public static bool operator!= (Vertex a, Vertex b)
        {
            return !(a == b);
        }
    }
}
