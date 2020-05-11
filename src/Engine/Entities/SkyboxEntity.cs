using Engine.Components;
using Engine.Entities;
using Engine.MathUtils;
using Engine.Render;
using OpenGL;

namespace Ulaid.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        public SkyboxEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f),
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 1)));
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Skybox/main.frag", ShaderType.FragmentShader),
                new Shader("Content/Shaders/Skybox/main.vert", ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Skybox");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            AddComponent(new MaterialComponent(new Material($"Content/Materials/{path}.mtl")));
            AddComponent(new MeshComponent($"Content/Models/{path}.obj"));
        }
    }
}