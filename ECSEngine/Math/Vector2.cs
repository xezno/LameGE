namespace ECSEngine.Math
{
    // TODO: Consider removing because OpenGL.Net.Math has built-in Vertex2f classes
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        // TODO: Add more mathematical operators for both other Vector2s and floats too

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

        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
}
