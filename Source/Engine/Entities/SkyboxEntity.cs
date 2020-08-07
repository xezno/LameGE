using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

namespace Engine.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        private TransformComponent transform;

        public SkyboxEntity()
        {
            transform = new TransformComponent(new Vector3d(0, 2f, -2f), new Vector3d(0, 0, 0), new Vector3d(1, 1, 1))   
            {
                ParentTransform = SceneManager.Instance.MainCamera.GetComponent<TransformComponent>() // TODO: Render this as a parent of every camera?
            };
            AddComponent(transform);
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Skybox/skybox.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Skybox/skybox.vert", Shader.Type.VertexShader)));
            AddComponent(new MaterialComponent(new Material($"Content/Materials/Skybox.mtl")));
            AddComponent(new MeshComponent($"Content/Models/Skybox.obj"));
        }
    }
}