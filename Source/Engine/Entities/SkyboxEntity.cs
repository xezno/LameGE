using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Quincy.Components;
using Quincy.Managers;

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
            AddComponent(new ShaderComponent("Content/Shaders/Skybox/skybox.frag", "Content/Shaders/Skybox/skybox.vert"));
            AddComponent(new ModelComponent($"Content/Models/Skybox.obj"));
        }
    }
}