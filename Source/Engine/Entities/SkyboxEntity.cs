using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

namespace Ulaid.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        private TransformComponent transform;

        public SkyboxEntity()
        {
            transform = new TransformComponent(new Vector3(0, 2f, -2f),
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 1));
            AddComponent(transform);
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Skybox/skybox.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Skybox/skybox.vert", Shader.Type.VertexShader)));
            AddMeshAndMaterialComponents("Skybox");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            AddComponent(new MaterialComponent(new Material($"Content/Materials/{path}.mtl")));
            AddComponent(new MeshComponent($"Content/Models/{path}.obj"));
        }

        public override void Update(float deltaTime)
        {
            transform.Position = SceneManager.Instance.mainCamera.Position; // big brain probably
        }
    }
}