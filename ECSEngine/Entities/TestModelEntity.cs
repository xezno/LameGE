using System;
using ECSEngine.Components;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL.CoreUI;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        private Material mainMaterial;
        public TestModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/level01");

            // mainMaterial.diffuseTexture = GenNoiseTexture();
        }

        private Texture2D GenNoiseTexture()
        {
            Random random = new Random();
            int seed = random.Next(0, int.MaxValue);
            // Generate test perlin noise texture
            int width = 256,
                height = 256;
            float scale = 50.0f;
            byte[] noiseValues = new byte[width * height];

            byte lowestVal = 0, highestVal = 0;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    byte noiseVal = Noise.PerlinNoiseAsByte(seed, (x / (float)width) * scale, (y / (float)height) * scale);
                    for (int i = 0; i < 3; i++)
                        noiseValues[x + (width * y) % noiseValues.Length] = noiseVal;

                    if (noiseVal > highestVal) highestVal = noiseVal;
                    if (noiseVal < lowestVal) lowestVal = noiseVal;
                }
            }


            // Normalize values
            int range = highestVal - lowestVal;
            for (int i = 0; i < noiseValues.Length; i++)
            {
                noiseValues[i] = (byte)(((noiseValues[i] - lowestVal) / (float)range) * 255);
            }

            // Convert to RGB
            byte[] noiseValuesRGB = new byte[width * height * 3];
            for (int i = 0; i < noiseValues.Length; i++)
            {
                for (int rgb = 0; rgb < 3; rgb++)
                    noiseValuesRGB[(i * 3) + rgb] = noiseValues[i];
            }
            var texture = new Texture2D(noiseValuesRGB, width, height);
            return texture;
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"{path}.obj"));
        }
    }
}
