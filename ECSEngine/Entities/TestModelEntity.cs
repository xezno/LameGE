using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Render;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        public TestModelEntity() : base()
        {
            // Add mesh component
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/level01");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            AddComponent(new MaterialComponent(new Material($"{path}.mtl")));
            AddComponent(new MeshComponent($"{path}.obj"));
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs) { }
    }
}
