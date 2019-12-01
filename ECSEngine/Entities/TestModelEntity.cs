using ECSEngine.Components;
using ECSEngine.Render;
using ECSEngine.MathUtils;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        private Material mainMaterial;
        public TestModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/PBRTest/MetalBall");
        }

        private Texture2D GenNoiseTexture()
        {
            // Generate test perlin noise texture
            int width = 32, height = 32;
            byte[] noiseValues = new byte[width * height * 3 /* r g b */];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    byte noiseVal = (byte)(Noise.PerlinNoise(0, x, y) * 255);
                    for (int i = 0; i < 3; i++)
                        noiseValues[x + (width * y) + i] = noiseVal;
                }
            }
            var texture = new Texture2D(noiseValues, width, height);
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
