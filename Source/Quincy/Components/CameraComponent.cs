using Engine.ECS.Components;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;
using System;

namespace Quincy.Components
{
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        [Range(0, 180)] public float FieldOfView { get; set; } = 90f;
        public float NearPlane { get; set; } = 0.1f;
        public float FarPlane { get; set; } = 2500f;

        public Vector2f Resolution { get; set; } = new Vector2f(GameSettings.GameResolutionX, GameSettings.GameResolutionY);

        private Matrix4x4f viewMatrix;
        public Matrix4x4f ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        private Matrix4x4f projMatrix;
        public Matrix4x4f ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public CameraComponent()
        {
            ProjMatrix = CreateInfReversedZProj(FieldOfView,
                Resolution.x / Resolution.y,
                NearPlane);
        }

        private Matrix4x4f CreateInfReversedZProj(float fov, float aspectRatio, float nearPlane)
        {
            float f = 1.0f / (float)Math.Tan(Angle.ToRadians(fov) / 2.0f);
            return new Matrix4x4f(f / aspectRatio, 0f, 0f, 0f,
                0f, f, 0f, 0f,
                0f, 0f, 0f, -1f,
                0f, 0f, nearPlane, 0f);
        }

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
