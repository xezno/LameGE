using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.MathUtils;
using OpenGL;
using Quaternion = ECSEngine.MathUtils.Quaternion;

namespace ECSEngine.Entities
{
    /// <summary>
    /// An entity which represents the camera in the world.
    /// </summary>
    public sealed class CameraEntity : Entity<CameraEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Camera;

        /// <summary>
        /// Gets the projection matrix from the camera component.
        /// </summary>
        public Matrix4x4f ProjMatrix => GetComponent<CameraComponent>().projMatrix;

        /// <summary>
        /// Gets the view matrix from the camera component.
        /// </summary>
        public Matrix4x4f ViewMatrix => GetComponent<CameraComponent>().viewMatrix;

        /// <summary>
        /// Gets or sets the camera's position from the transform component.
        /// </summary>
        public Vector3 Position
        {
            get { return GetComponent<TransformComponent>().Position; }
            set { GetComponent<TransformComponent>().Position = value; }
        }

        /// <summary>
        /// Gets or sets the camera's rotation (as an euler angle) from the transform component.
        /// </summary>
        public Vector3 RotationEuler
        {
            get { return GetComponent<TransformComponent>().RotationEuler; }
            set { GetComponent<TransformComponent>().RotationEuler = value; }
        }

        /// <summary>
        /// Constructs the camera entity with a transform component and a camera component.
        /// </summary>
        public CameraEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new CameraComponent());
        }
    }
}
