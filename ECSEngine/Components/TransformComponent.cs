using ECSEngine.Math;
using Quaternion = ECSEngine.Math.Quaternion;

using OpenGL;

namespace ECSEngine.Components
{
    /// <summary>
    /// A container component for the entity's position and rotation.
    /// </summary>
    public class TransformComponent : Component<TransformComponent>
    {
        /// <summary>
        /// The entity's position.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The entity's rotation.
        /// </summary>
        // public Quaternion rotation; // TODO

        public Vector3 rotationEuler;

        /// <summary>
        /// The entity's scale.
        /// </summary>
        public Vector3 scale;

        public Matrix4x4f matrix
        {
            get
            {
                Matrix4x4f temp = Matrix4x4f.Identity;
                // Vector3 euler = rotation.ToEulerAngles();
                temp.RotateX(rotationEuler.x);
                temp.RotateX(rotationEuler.y);
                temp.RotateX(rotationEuler.z);
                temp.Translate(position.x, position.y, position.z);
                temp.Scale(scale.x, scale.y, scale.z);
                return temp;
            }
        }

        /// <summary>
        /// Construct a new TransformComponent with the parameters specified.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="rotation">The entity's rotation.</param>
        /// <param name="scale">The entity's scale.</param>
        public TransformComponent(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.position = position;
            this.rotationEuler = rotation.ToEulerAngles(); // TODO
            this.scale = scale;
        }
    }
}
