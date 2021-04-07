using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils;
using Engine.Utils.MathUtils;
using System.Numerics;

namespace Engine.Entities
{
    public sealed class SkyboxEntity : Entity<SkyboxEntity>
    {
        private TransformComponent transform;

        public SkyboxEntity()
        {
            var fs = ServiceLocator.FileSystem;

            transform = new TransformComponent(new Vector3(0, 2f, -2f), new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            AddComponent(transform);
            AddComponent(new ShaderComponent(fs.GetAsset("/Shaders/Skybox/skybox.frag"),
                                             fs.GetAsset("/Shaders/Skybox/skybox.vert")));
            AddComponent(new ModelComponent(fs.GetAsset("/Models/Skybox.obj")));
        }
    }
}