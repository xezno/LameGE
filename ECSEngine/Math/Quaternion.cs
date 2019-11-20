namespace ECSEngine.Math
{
    public struct Quaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Quaternion identity = new Quaternion(0, 0, 0, 1);

        public static Quaternion FromEulerAngles(Vector3 eulerAngles)
        {
            float halfCosX = (float)System.Math.Cos(eulerAngles.x) / 2.0f;
            float halfCosY = (float)System.Math.Cos(eulerAngles.y) / 2.0f;
            float halfCosZ = (float)System.Math.Cos(eulerAngles.z) / 2.0f;
            float halfSinX = (float)System.Math.Cos(eulerAngles.x) / 2.0f;
            float halfSinY = (float)System.Math.Cos(eulerAngles.y) / 2.0f;
            float halfSinZ = (float)System.Math.Cos(eulerAngles.z) / 2.0f;

            return new Quaternion(halfCosX * halfCosY * halfCosZ + halfSinX * halfSinY * halfSinZ,
                halfSinX * halfCosY * halfCosZ - halfCosX * halfSinY * halfSinZ,
                halfCosX * halfSinY * halfCosZ + halfSinX * halfCosY * halfSinZ,
                halfCosX * halfCosY * halfSinZ - halfSinX * halfSinY * halfCosZ);
        }

        public Vector3 ToEulerAngles()
        {
            return new Vector3(
                (float)System.Math.Atan2(2 * ((w * x) + (y * z)), 1 - 2 * ((x * x) + (y * y))),
                (float)System.Math.Asin(2 * ((w * y) - (z * x))),
                (float)System.Math.Atan2(2 * ((w * z) + (x * y)), 1 - 2 * ((y * y) + (z * z)))
            );
        }

        // TODO: Add mathematical operators

    }
}
