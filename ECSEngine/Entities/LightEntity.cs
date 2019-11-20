using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Render;
using ECSEngine.Systems;

namespace ECSEngine.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        private TransformComponent transformComponent;
        public LightEntity()
        {
            // Add mesh component
            transformComponent = new TransformComponent(new Vector3(0, 2f, -2f), Quaternion.identity, new Vector3(1, 1, 1));
            AddComponent(transformComponent);

            // See values from http://wiki.ogre3d.org/-Point+Light+Attenuation
            AddComponent(new LightComponent(600, 0.007f, 0.0002f));
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            switch (eventType)
            {
                case Event.GameStart:
                    Program.game.GetSystem<ImGuiSystem>().AddSerializableObject(transformComponent); // this is also terrible
                    break;
            }
        }
    }
}