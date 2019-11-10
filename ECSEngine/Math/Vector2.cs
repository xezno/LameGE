namespace ECSEngine.Math
{
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

        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x,
                                                                                a.y * b.y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x,
                                                                                a.y - b.y);
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x,
                                                                                a.y / b.y);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x,
                                                                              a.y + b.y);

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
