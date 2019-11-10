using OpenGL;

namespace ECSEngine.Math
{
    // TODO: Consider removing because OpenGL.Net.Math has built-in Vertex3f classes
    public struct Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // TODO: Add more mathematical operators for both other Vector3s and floats too

        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, 
                                                                                a.y * b.y, 
                                                                                a.z * b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x,
                                                                                a.y - b.y,
                                                                                a.z - b.z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x,
                                                                                a.y / b.y,
                                                                                a.z / b.z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x,
                                                                                a.y + b.y,
                                                                                a.z + b.z);

        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b,
                                                                            a.y * b,
                                                                            a.z * b);
        public static Vector3 operator -(Vector3 a, float b) => new Vector3(a.x - b,
                                                                            a.y - b,
                                                                            a.z - b);
        public static Vector3 operator /(Vector3 a, float b) => new Vector3(a.x / b,
                                                                            a.y / b,
                                                                            a.z / b);
        public static Vector3 operator +(Vector3 a, float b) => new Vector3(a.x + b,
                                                                            a.y + b,
                                                                            a.z + b);

        public static implicit operator Vertex3f(Vector3 a)
        {
            return new Vertex3f(a.x, a.y, a.z);
        }

        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }
    }
}
