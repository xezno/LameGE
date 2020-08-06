using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;

namespace Engine.Components
{
    /// <summary>
    /// Set up basic camera matrices, and perform relevant operations on them.
    /// </summary>
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        [Range(0, 180)] public float FieldOfView { get; set; } = 90f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 2500f;

        // TODO: Vector2 integer-only?
        public Vector2f Resolution { get; set; } = new Vector2f(GameSettings.GameResolutionX, GameSettings.GameResolutionY);

        private Matrix4x4f viewMatrix;
        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        private Matrix4x4f projMatrix;
        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public Framebuffer Framebuffer { get; set; }

        /// <summary>
        /// Construct an instance of CameraComponent, setting up the projection matrix in the process.
        /// </summary>
        public CameraComponent()
        {
            ProjMatrix = Matrix4x4f.Perspective(FieldOfView,
                Resolution.x / Resolution.y,
                NearPlane,
                FarPlane);

            Framebuffer = new Framebuffer((int)Resolution.x, (int)Resolution.y);
        }

        /// <summary>
        /// Update the CameraComponent's matrices where required.
        /// </summary>
        public override void Update(float deltaTime)
        {
            // TODO: Better double->float conversion
            var transformComponent = GetComponent<TransformComponent>();
            viewMatrix = Matrix4x4f.Identity;
            viewMatrix.RotateX((float)transformComponent.RotationEuler.x);
            viewMatrix.RotateY((float)transformComponent.RotationEuler.y);
            viewMatrix.RotateZ((float)transformComponent.RotationEuler.z);
            viewMatrix *= (Matrix4x4f.LookAtDirection(
                new Vertex3f((float)transformComponent.Position.x, (float)transformComponent.Position.y, (float)transformComponent.Position.z),
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f)));
        }
    }
}
