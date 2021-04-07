using BepuPhysics;
using Engine.ECS.Components;

namespace Engine.Components
{
    public class BaseColliderComponent : Component<BaseColliderComponent>
    {
        public bool IsStatic { get; set; }
        public bool IsKinematic { get; set; }
        public BodyHandle BodyHandle { get; set; }
        public StaticHandle StaticHandle { get; set; }
        public float Mass { get; set; } = 1.0f;
    }
}
