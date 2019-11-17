using ECSEngine.Attributes;

namespace ECSEngine.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        private float luminosity;
        private float falloff;
        private float range;

        public LightComponent(float luminosity, float falloff, float range)
        {
            this.luminosity = luminosity;
            this.falloff = falloff;
            this.range = range;
        }

        public override void Update()
        {
            
        }

        public override void Render()
        {
            
        }
    }
}
