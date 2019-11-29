namespace ECSEngine.Math
{
    public static class Noise
    {
        private static float[,,] gradientValues = new float[255,255,2];

        private static float Lerp(float a, float b, float t) => (1.0f - t) * a + (t * b);

        private static float DotGridGradient(int ix, int iy, float x, float y)
        {
            float dx = x - ix;
            float dy = y - iy;

            return dx * gradientValues[ix, iy, 0] + dy * gradientValues[ix, iy, 1];
        }

        private static void CalculateGradientValues()
        {
            if (gradientValues[0, 0, 0].Equals(0.0f) && gradientValues[254, 254, 1].Equals(0.0f))
            {

            }
        }

        public static float PerlinNoise(float x, float y)
        {
            int x0 = (int)x;
            int x1 = x0 + 1;

            int y0 = (int)y;
            int y1 = y0 + 1;

            float sx = x - x0;
            float sy = y - y0;

            float n0, n1, ix0, ix1;
            n0 = DotGridGradient(x0, y0, x, y);
            n1 = DotGridGradient(x1, y0, x, y);
            ix0 = Lerp(n0, n1, sx);

            n0 = DotGridGradient(x0, y1, x, y);
            n1 = DotGridGradient(x1, y1, x, y);
            ix1 = Lerp(n0, n1, sx);

            return Lerp(ix0, ix1, sy);
        }
    }
}
