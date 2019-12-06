using ECSEngine.Components;
using ECSEngine.MathUtils;

using OpenGL;

namespace ECSEngine.Entities
{
    /// <summary>
    /// An entity which represents the camera in the world.
    /// </summary>
    public sealed class CameraEntity : Entity<CameraEntity>
    {
        /// <summary>
        /// Gets the projection matrix from the camera component.
        /// </summary>
        public Matrix4x4f projMatrix => GetComponent<CameraComponent>().projMatrix;

        /// <summary>
        /// Gets the view matrix from the camera component.
        /// </summary>
        public Matrix4x4f viewMatrix => GetComponent<CameraComponent>().viewMatrix;

        /// <summary>
        /// Gets or sets the camera's position from the transform component.
        /// </summary>
        public Vector3 position
        {
            get { return GetComponent<TransformComponent>().position; }
            set { GetComponent<TransformComponent>().position = value; }
        }

        /// <summary>
        /// Gets or sets the camera's rotation (as an euler angle) from the transform component.
        /// </summary>
        public Vector3 rotationEuler
        {
            get { return GetComponent<TransformComponent>().rotationEuler; }
            set { GetComponent<TransformComponent>().rotationEuler = value; }
        }

        /// <summary>
        /// Constructs the camera entity with a transform component and a camera component.
        /// </summary>
        public CameraEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), MathUtils.Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new CameraComponent());
        }
    }
}
