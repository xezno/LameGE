using Engine.ECS.Components;
using Engine.Renderer.GL.Components;
using Engine.Utils;
using Engine.Utils.Attributes;
using OpenGL;

namespace Engine.Components
{
    /// <summary>
    /// Set up basic camera matrices, and perform relevant operations on them.
    /// </summary>
    [Requires(typeof(TransformComponent))]
    public class CameraComponent : Component<CameraComponent>
    {
        public Matrix4x4f viewMatrix, projMatrix;
        [Range(0, 180)]
        public float fieldOfView = 90.0f,
            nearPlane = 0.1f,
            farPlane = 2500f;

        /// <summary>
        /// Construct an instance of CameraComponent, setting up the projection matrix in the process.
        /// </summary>
        public CameraComponent()
        {
            projMatrix = Matrix4x4f.Perspective(fieldOfView,
                GameSettings.GameResolutionX / (float)GameSettings.GameResolutionY,
                nearPlane,
                farPlane);
        }

        /// <summary>
        /// Update the CameraComponent's matrices where required.
        /// </summary>
        public override void Update(float deltaTime)
        {
            var transformComponent = GetComponent<TransformComponent>();
            viewMatrix = Matrix4x4f.Identity;
            viewMatrix.RotateX(transformComponent.RotationEuler.x);
            viewMatrix.RotateY(transformComponent.RotationEuler.y);
            viewMatrix.RotateZ(transformComponent.RotationEuler.z);
            viewMatrix *= (Matrix4x4f.LookAtDirection(
                new Vertex3f(transformComponent.Position.x, transformComponent.Position.y, transformComponent.Position.z),
                new Vertex3f(0f, 0f, -1f),
                new Vertex3f(0f, 1f, 0f)));
        }
    }
}
