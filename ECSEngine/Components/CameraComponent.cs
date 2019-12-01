using ECSEngine.Attributes;

using OpenGL;

namespace ECSEngine.Components
{
    /// <summary>
    /// Set up basic camera matrices, and perform relevant operations on them.
    /// </summary>
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public Matrix4x4f viewMatrix, projMatrix;
        public float fieldOfView = 90.0f,
            nearPlane = 0.1f,
            farPlane = 50f;

        /// <summary>
        /// Construct an instance of CameraComponent, setting up the projection matrix in the process.
        /// </summary>
        public CameraComponent()
        {
            projMatrix = Matrix4x4f.Perspective(fieldOfView,
                RenderSettings.Default.GameResolutionX / (float)RenderSettings.Default.GameResolutionY,
                nearPlane,
                farPlane);
        }

        /// <summary>
        /// Update the CameraComponent's matrices where required.
        /// </summary>
        public override void Update(float deltaTime)
        {
            viewMatrix = Matrix4x4f.LookAtDirection(GetComponent<TransformComponent>().position,
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f));
        }
    }
}
