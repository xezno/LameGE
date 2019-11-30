using System;

namespace ECSEngine.Math
{
    public static class Noise
    {
        private static Vector2[,] gradientValues = new Vector2[255,255];

        private static float Lerp(float a, float b, float t) => (1.0f - t) * a + (t * b);

        private static float DotGridGradient(int ix, int iy, float x, float y)
        {
            float dx = x - ix;
            float dy = y - iy;

            return dx * gradientValues[ix, iy].x + dy * gradientValues[ix, iy].y;
        }

        private static void CalculateGradientValues(int seed)
        {
            Random random = new Random(seed);
            for (int x = 0; x < gradientValues.GetLength(0); x++)
            {
                for (int y = 0; y < gradientValues.GetLength(1); y++)
                {
                    gradientValues[x, y] = new Vector2(
                        ((float)random.NextDouble() * 2) - 1,
                        ((float)random.NextDouble() * 2) - 1
                        );
                }
            }
        }

        public static float PerlinNoise(int seed, float x, float y)
        {
            CalculateGradientValues(seed);

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
