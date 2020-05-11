namespace Engine.Utils.MathUtils
{
    // TODO: Consider removing because OpenGL.Net.Math has built-in Vertex2f classes
    public struct Vector2
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
        /// Construct a <see cref="Vector2"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 Multiply(Vector2 a, Vector2 b) => new Vector2(a.x * b.x,
                                                                            a.y * b.y);
        public static Vector2 Subtract(Vector2 a, Vector2 b) => new Vector2(a.x - b.x,
                                                                            a.y - b.y);
        public static Vector2 Divide(Vector2 a, Vector2 b) => new Vector2(a.x / b.x,
                                                                            a.y / b.y);
        public static Vector2 Add(Vector2 a, Vector2 b) => new Vector2(a.x + b.x,
                                                                            a.y + b.y);

        public static Vector2 operator *(Vector2 a, Vector2 b) => Multiply(a, b);
        public static Vector2 operator -(Vector2 a, Vector2 b) => Subtract(a, b);
        public static Vector2 operator /(Vector2 a, Vector2 b) => Divide(a, b);
        public static Vector2 operator +(Vector2 a, Vector2 b) => Add(a, b);

        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.x * b,
                                                                            a.y * b);
        public static Vector2 operator -(Vector2 a, float b) => new Vector2(a.x - b,
                                                                            a.y - b);
        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.x / b,
                                                                            a.y / b);
        public static Vector2 operator +(Vector2 a, float b) => new Vector2(a.x + b,
                                                                            a.y + b);
        /// <summary>
        /// Get all values within the <see cref="Vector2"/> as a string.
        /// </summary>
        /// <returns>Both coordinates (<see cref="x"/> and <see cref="y"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
}
