using ECSEngine.Components;
using ECSEngine.Math;

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
        /// Constructs the camera entity with a transform component and a camera component.
        /// </summary>
        public CameraEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), ECSEngine.Math.Quaternion.identity));
            AddComponent(new CameraComponent());
        }
    }
}
