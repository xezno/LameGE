using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Render;
using ECSEngine.Managers;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        private Material mainMaterial;
        private TransformComponent transformComponent;
        public TestModelEntity()
        {
            // Add mesh component
            transformComponent = new TransformComponent(new Vector3(0, 2f, -2f), Quaternion.identity, new Vector3(1, 1, 1));
            AddComponent(transformComponent);

            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/PBRTest/MetalBall");

            // mainMaterial.diffuseTexture = GenNoiseTexture();
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

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            switch (eventType)
            {
                case Event.GameStart:
                    ImGuiManager.instance.AddSerializableObject(mainMaterial); // this is terrible
                    ImGuiManager.instance.AddSerializableObject(transformComponent); // this is also terrible
                    break;
            }
        }
    }
}
