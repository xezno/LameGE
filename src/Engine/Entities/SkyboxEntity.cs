using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

namespace Ulaid.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        public SkyboxEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f),
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 1)));
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Skybox/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Skybox/main.vert", Shader.Type.VertexShader)));
            AddMeshAndMaterialComponents("Skybox");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            AddComponent(new MaterialComponent(new Material($"Content/Materials/{path}.mtl")));
            AddComponent(new MeshComponent($"Content/Models/{path}.obj"));
        }
    }
}