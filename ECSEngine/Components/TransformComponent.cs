using ECSEngine.MathUtils;
using OpenGL;
using Quaternion = ECSEngine.MathUtils.Quaternion;

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

        public Matrix4x4f Matrix
        {
            get
            {
                var temp = Matrix4x4f.Identity;
                temp.Translate(position.x, position.y, position.z);
                temp.RotateX(rotationEuler.x);
                temp.RotateY(rotationEuler.y);
                temp.RotateZ(rotationEuler.z);
                temp.Scale(scale.x, scale.y, scale.z);
                return temp;
            }
        }

        public Vector3 Forward
        {
            get
            {
                var inverseColumn = Matrix.Inverse.Column2;
                return new Vector3(inverseColumn.x, inverseColumn.y, inverseColumn.z).Normalized;
            }
        }

        public Vector3 Up
        {
            get
            {
                var inverseColumn = Matrix.Inverse.Column1;
                return new Vector3(inverseColumn.x, inverseColumn.y, inverseColumn.z).Normalized;
            }
        }

        public Vector3 Right
        {
            get
            {
                var inverseColumn = Matrix.Inverse.Column0;
                return new Vector3(inverseColumn.x, inverseColumn.y, inverseColumn.z).Normalized;
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
            rotationEuler = rotation.ToEulerAngles(); // TODO
            this.scale = scale;
        }

        /// <summary>
        /// Construct a new TransformComponent with the parameters specified.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="rotationEuler">The entity's rotation.</param>
        /// <param name="scale">The entity's scale.</param>
        public TransformComponent(Vector3 position, Vector3 rotationEuler, Vector3 scale)
        {
            this.position = position;
            this.rotationEuler = rotationEuler; // TODO
            this.scale = scale;
        }
    }
}
