using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Render;

namespace ECSEngine.Entities
{
    public class TestModelEntity : Entity<TestModelEntity>
    {
        public TestModelEntity() : base()
        {
            // Add mesh component
            AddComponent(new ShaderComponent(new Shader("Content/frag.glsl", OpenGL.ShaderType.FragmentShader), new Shader("Content/vert.glsl", OpenGL.ShaderType.VertexShader)));
            AddComponent(new MaterialComponent(Material.LoadAllFromFile("Content/level01.mtl")));
            AddComponent(new MeshComponent("Content/level01.obj", new Texture2D("Content/texture.jpg"))); // TODO: Merge mesh loading & material loading
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            Debug.Log($"Received event {eventType.ToString()}");
        }
    }
}
