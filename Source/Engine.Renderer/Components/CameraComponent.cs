using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Utils;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Numerics;

namespace Engine.Renderer.Components
{
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public Framebuffer Framebuffer { get; private set; }

        private float nearPlane = 0.1f;
        private float farPlane = 2500f;
        private float fieldOfView = 90f;

        private Matrix4x4 viewMatrix;
        private Matrix4x4 projMatrix;

        [Range(0, 180)] public float FieldOfView { get => fieldOfView; set { fieldOfView = value; CreateProjectionMatrix(); } }
        public float NearPlane { get => nearPlane; set { nearPlane = value; CreateProjectionMatrix(); } }
        public float FarPlane { get => farPlane; set { farPlane = value; CreateProjectionMatrix(); } }


        private Vector2? resolution = null;
        public Vector2 Resolution
        {
            get => resolution ?? new Vector2(GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            set
            {
                resolution = value;
                CreateFramebuffer();
                CreateProjectionMatrix();
            }
        }

        public Matrix4x4 ViewMatrix { get => viewMatrix; set => viewMatrix = value; }

        public Matrix4x4 ProjMatrix { get => projMatrix; set => projMatrix = value; }

        public CameraComponent()
        {
            CreateProjectionMatrix();
            CreateFramebuffer();
        }

        private void CreateProjectionMatrix()
        {
            ProjMatrix = CreateInfReversedZProj(FieldOfView,
                Resolution.X / Resolution.Y,
                NearPlane);
        }

        private void CreateFramebuffer()
        {
            Framebuffer = new Framebuffer((int)Resolution.X, (int)Resolution.Y);
        }

        private Matrix4x4 CreateInfReversedZProj(float fov, float aspectRatio, float nearPlane)
        {
            float f = 1.0f / (float)Math.Tan(Angle.ToRadians(fov) / 2.0f);
            return new Matrix4x4(f / aspectRatio, 0f, 0f, 0f,
                0f, f, 0f, 0f,
                0f, 0f, 0f, -1f,
                0f, 0f, nearPlane, 0f);
        }

        public override void Update(float deltaTime)
        {
            var transformComponent = GetComponent<TransformComponent>();
            viewMatrix = Matrix4x4.Identity;
            viewMatrix *= (Matrix4x4Extensions.LookAtDirection(
                transformComponent.Position,
                new Vector3(0, 0, -1f),
                new Vector3(0, 1f, 0)));
            viewMatrix *= Matrix4x4.CreateFromQuaternion(transformComponent.Rotation);
        }

        public override void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            switch (notifyType)
            {
                case NotifyType.WindowResized:
                    if (resolution != null)
                        break;

                    // Assume res is correct, just refresh everything.
                    CreateProjectionMatrix();
                    CreateFramebuffer();
                    break;
            }
            base.OnNotify(notifyType, notifyArgs);
        }
    }
}
