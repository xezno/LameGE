using Engine.ECS.Entities;
using Engine.Renderer.Components;

namespace Engine.Renderer.Entities
{
    public class SkyboxEntity : Entity<SkyboxEntity>
    {
        public SkyboxEntity(string hdriPath)
        {
            AddComponent(new SkyboxComponent(hdriPath));
        }
    }
}
