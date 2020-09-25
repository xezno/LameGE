using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace Quincy.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Lightbulb;

        private TransformComponent transformComponent;
        private LightComponent lightComponent;

        public LightEntity()
        {
            transformComponent = new TransformComponent(new Vector3d(0, 2f, 0f), Quaternion.identity, new Vector3d(1, 1, 1) * 0.5f);
            AddComponent(transformComponent);

            // See values from http://wiki.ogre3d.org/-Point+Light+Attenuation
            lightComponent = new LightComponent(transformComponent.Position);
            AddComponent(lightComponent);
        }
    }
}