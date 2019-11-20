using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Render;
using ECSEngine.Systems;

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
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"{path}.obj"));
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            switch (eventType)
            {
                case Event.GameStart:
                    ImGuiSystem.instance.AddSerializableObject(mainMaterial); // this is terrible
                    ImGuiSystem.instance.AddSerializableObject(transformComponent); // this is also terrible
                    break;
            }
        }
    }
}
