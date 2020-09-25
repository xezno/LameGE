using Engine.ECS.Components;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;

namespace Quincy.Components
{
    [Requires(typeof(TransformComponent))]
    public sealed class LightComponent : Component<LightComponent>
    {
        public LightComponent(Vector3d position)
        {
            Position = position;
            var size = 20f;
            var farPlane = 200f;
            ProjMatrix = Matrix4x4f.Ortho(-size, size, -size, size, 0.1f, farPlane);
            ViewMatrix = Matrix4x4f.Identity;
            ShadowMap = new ShadowMap(4096, 4096);
        }

        public Vector3d Position { get; set; }
        public Matrix4x4f ViewMatrix { get; set; }
        public Matrix4x4f ProjMatrix { get; set; }
        public ShadowMap ShadowMap { get; set; }

        public override void Render()
        {
            ViewMatrix = Matrix4x4f.LookAt(new Vertex3f(
                (float)Position.x,
                (float)Position.y,
                (float)Position.z), new Vertex3f(0f, 0f, 0f), new Vertex3f(0f, 1f, 0f));
        }
    }
}
