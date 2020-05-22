using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.Attributes;
using OpenGL;

namespace Engine.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        public float range;
        public readonly float constant = 1.0f;
        public float linear;
        public float quadratic;
        public ShadowMap shadowMap;
        public Vertex3f lookAt = Vertex3f.Zero;

        private float nearPlane = 0.03f;
        private float farPlane = 10.0f;

        public Matrix4x4f projMatrix => Matrix4x4f.Ortho(-10.0f, 10.0f, -10.0f, 10.0f, nearPlane, farPlane);
        public Matrix4x4f viewMatrix
        {
            get
            {
                var transformComponent = GetComponent<TransformComponent>();
                return Matrix4x4f.LookAt(new Vertex3f(transformComponent.Position.x, transformComponent.Position.y, transformComponent.Position.z),
                    lookAt,
                    new Vertex3f(0, 1, 0)
                );
            }
        }

        public Matrix4x4f lightMatrix => projMatrix * viewMatrix;

        public LightComponent(float range, float linear, float quadratic)
        {
            this.range = range;
            this.linear = linear;
            this.quadratic = quadratic;

            shadowMap = new ShadowMap(GameSettings.ShadowMapX, GameSettings.ShadowMapY);
        }
    }
}
