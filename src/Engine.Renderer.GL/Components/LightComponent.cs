using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Utils.Attributes;

namespace Engine.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        public float range;
        public readonly float constant = 1.0f;
        public float linear;
        public float quadratic;

        public LightComponent(float range, float linear, float quadratic)
        {
            this.range = range;
            this.linear = linear;
            this.quadratic = quadratic;
        }
    }
}
