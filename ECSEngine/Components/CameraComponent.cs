using OpenGL;

namespace ECSEngine.Components
{
    public class CameraComponent : Component<CameraComponent>
    {
        public float rotationAngle, distance = 2f;
        public Matrix4x4f viewMatrix, projMatrix;
        public CameraComponent()
        {
            viewMatrix = Matrix4x4f.LookAt(new Vertex3f(0f, 0f, 1f),
                new Vertex3f(0f, 0f, 0f),
                new Vertex3f(0f, 1f, 0f));

            projMatrix = Matrix4x4f.Perspective(90f, 
                (float)RenderSettings.Default.GameResolutionX / (float)RenderSettings.Default.GameResolutionY, 
                0.1f, 
                10f);
        }

        public override void Update()
        {

        }

        public override void Render()
        {
            rotationAngle += 0.016f;
            viewMatrix = Matrix4x4f.LookAt(new Vertex3f((float)System.Math.Sin(rotationAngle) * distance, 0f, (float)System.Math.Cos(rotationAngle) * distance),
                new Vertex3f(0f, 0f, 0f),
                new Vertex3f(0f, 1f, 0f));
        }
    }
}
