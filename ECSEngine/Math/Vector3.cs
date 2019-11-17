using OpenGL;

namespace ECSEngine.Math
{
    public struct Vector3
    {
        /// <summary>
        /// The point at which the vector resides on the X axis
        /// </summary>
        public float x;

        /// <summary>
        /// The point at which the vector resides on the Y axis
        /// </summary>
        public float y;

        /// <summary>
        /// The point at which the vector resides on the Z axis
        /// </summary>
        public float z;

        /// <summary>
        /// Construct a <see cref="Vector3"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        /// <param name="z">The initial z coordinate</param>
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

        /// <summary>
        /// Get all values within the <see cref="Vector3"/> as a string.
        /// </summary>
        /// <returns>All coordinates (<see cref="x"/>, <see cref="y"/> and <see cref="z"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }
    }
}
