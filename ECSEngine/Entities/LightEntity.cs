using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Managers;

namespace ECSEngine.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        private TransformComponent transformComponent;
        private LightComponent lightComponent;
        public LightEntity()
        {
            // Add mesh component
            transformComponent = new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1));
            AddComponent(transformComponent);

            // See values from http://wiki.ogre3d.org/-Point+Light+Attenuation
            lightComponent = new LightComponent(600, 0.007f, 0.0002f);
            AddComponent(lightComponent);
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            switch (eventType)
            {
                case Event.GameStart:
                    ImGuiManager.instance.AddSerializableObject(transformComponent, "Light");
                    break;
            }
        }

        public void Bind(ShaderComponent shaderComponent)
        {
            shaderComponent.SetVariable("light.pos", transformComponent.position);
            shaderComponent.SetVariable("light.range", lightComponent.range);
            shaderComponent.SetVariable("light.linear", lightComponent.linear);
            shaderComponent.SetVariable("light.quadratic", lightComponent.quadratic);
            shaderComponent.SetVariable("light.constant", lightComponent.constant);
        }
    }
}