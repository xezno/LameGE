using Engine.Utils.MathUtils;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Engine.Renderer
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Vertex
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 TexCoords { get; set; }
        public Vector3 Tangent { get; set; }
        public Vector3 BiTangent { get; set; }

        public static bool operator ==(Vertex a, Vertex b)
        {
            return (a.Position == b.Position) && (a.Normal == b.Normal) && (a.TexCoords == b.TexCoords) && (a.Tangent == b.Tangent) && (a.BiTangent == b.BiTangent);
        }

        public static bool operator !=(Vertex a, Vertex b)
        {
            return !(a == b);
        }
    }
}
