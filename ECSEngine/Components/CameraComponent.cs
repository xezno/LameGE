using ECSEngine.Attributes;

using OpenGL;

namespace ECSEngine.Components
{
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public float rotationAngle, distance = 2f;
        public Matrix4x4f viewMatrix, projMatrix;
        public float fieldOfView = 90.0f;
        public float rotationSpeed = 4;
        public CameraComponent()
        {
            projMatrix = Matrix4x4f.Perspective(fieldOfView,
                (float)RenderSettings.Default.GameResolutionX / (float)RenderSettings.Default.GameResolutionY,
                0.1f,
                50f);
        }

        public override void Update()
        {
            rotationAngle += 0.16f * rotationSpeed;
            viewMatrix = Matrix4x4f.LookAtDirection(GetComponent<TransformComponent>().position,
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f));
            viewMatrix.RotateY(rotationAngle);
            //viewMatrix.RotateX(rotationAngle);
            //viewMatrix.RotateZ(rotationAngle);
        }

        public override void Render()
        {
        }
    }
}
