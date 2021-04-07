using Engine.ECS.Components;
using Engine.Utils.Attributes;
using OpenGL;
using System.Numerics;

namespace Engine.Renderer.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        private Matrix4x4 viewMatrix;

        public LightComponent()
        {
            var size = 20f;
            var farPlane = 200f;
            ProjMatrix = Matrix4x4.CreateOrthographicOffCenter(-size, size, -size, size, 0.1f, farPlane);
            ViewMatrix = Matrix4x4.Identity;
            ShadowMap = new ShadowMap(4096, 4096);
        }

        public Matrix4x4 ViewMatrix { get => viewMatrix; set => viewMatrix = value; }
        public Matrix4x4 ProjMatrix { get; set; }
        public ShadowMap ShadowMap { get; set; }

        public override void Update(float deltaTime)
        {
            var transformComponent = GetComponent<TransformComponent>();
            viewMatrix = Matrix4x4.Identity;
            viewMatrix *= Matrix4x4.CreateFromQuaternion(transformComponent.Rotation);
            viewMatrix *= Matrix4x4.CreateLookAt(new Vector3(
                (float)transformComponent.Position.X,
                (float)transformComponent.Position.Y,
                (float)transformComponent.Position.Z), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f));
        }
    }
}
