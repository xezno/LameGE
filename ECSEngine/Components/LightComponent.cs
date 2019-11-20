using ECSEngine.Attributes;

namespace ECSEngine.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        private float range;
        private float constant = 1.0f;
        private float distance;
        private float quadratic;

        public LightComponent(float range, float distance, float quadratic)
        {
            this.range = range;
            this.distance = distance;
            this.quadratic = quadratic;
        }

        public override void Update()
        {
            
        }

        public override void Render()
        {
            
        }
    }
}
