namespace ECSEngine.Math
{
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
        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }
    }
}
