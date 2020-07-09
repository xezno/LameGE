using Engine.Assets;
using Engine.Components;
using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

namespace Engine.Renderer.GL.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Lightbulb;

        private TransformComponent transformComponent;
        private LightComponent lightComponent;

        public LightEntity()
        {
            transformComponent = new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1) * 0.5f);
            AddComponent(transformComponent);

            // Add mesh components for visualisation 
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Unlit/unlit.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Unlit/unlit.vert", Shader.Type.VertexShader)));
            AddComponent(new MaterialComponent(new Material($"Content/Materials/cube.mtl")));
            // AddComponent(new MeshComponent($"Content/Models/cube.obj"));

            // See values from http://wiki.ogre3d.org/-Point+Light+Attenuation
            lightComponent = new LightComponent(600, 0.007f, 0.0002f);
            AddComponent(lightComponent);
        }

        public void Bind(ShaderComponent shaderComponent)
        {
            shaderComponent.SetVariable("light.pos", transformComponent.Position);
            shaderComponent.SetVariable("light.range", lightComponent.range);
            shaderComponent.SetVariable("light.linear", lightComponent.linear);
            shaderComponent.SetVariable("light.quadratic", lightComponent.quadratic);
            shaderComponent.SetVariable("light.constant", lightComponent.constant);

            lightComponent.shadowMap.BindTexture();
            shaderComponent.SetVariable("shadowMap", 1);
            shaderComponent.SetVariable("lightMatrix", lightComponent.lightMatrix);
        }
    }
}