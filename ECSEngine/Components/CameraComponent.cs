using ECSEngine.Attributes;

using OpenGL;

namespace ECSEngine.Components
{
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public Matrix4x4f viewMatrix, projMatrix;
        public float rotationAngle, fieldOfView = 90.0f, rotationSpeed = 4, nearPlane = 0.1f, farPlane = 50f;
        public CameraComponent()
        {
            projMatrix = Matrix4x4f.Perspective(fieldOfView,
                RenderSettings.Default.GameResolutionX / (float)RenderSettings.Default.GameResolutionY,
                nearPlane,
                farPlane);
        }

        public override void Update()
        {
            rotationAngle += 0.16f * rotationSpeed;
            viewMatrix = Matrix4x4f.LookAtDirection(GetComponent<TransformComponent>().position,
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f));
            viewMatrix.RotateY(rotationAngle);
        }

        public override void Render()
        {
        }
    }
}
