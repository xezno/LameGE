using Engine.MathUtils;
using OpenGL;
using System;
using Quaternion = Engine.MathUtils.Quaternion;

namespace Engine.Components
{
    /// <summary>
    /// A container component for the entity's position and rotation.
    /// </summary>
    public class TransformComponent : Component<TransformComponent>
    {
        private Vector3 position;

        /// <summary>
        /// The entity's position.
        /// </summary>
        public Vector3 Position { get => position; set => position = value; }

        // public Quaternion rotation; // TODO
        private Vector3 rotationEuler;

        /// <summary>
        /// The entity's rotation.
        /// </summary>
        public Vector3 RotationEuler { 
            get => rotationEuler;
            set => rotationEuler = value % 360f;
        }

        private Vector3 scale;

        /// <summary>
        /// The entity's scale.
        /// </summary>
        public Vector3 Scale { get => scale; set => scale = value; }

        public Matrix4x4f Matrix
        {
            get
            {
                var temp = Matrix4x4f.Identity;
                temp.Translate(Position.x, Position.y, Position.z);
                temp.RotateX(rotationEuler.x);
                temp.RotateY(rotationEuler.y);
                temp.RotateZ(rotationEuler.z);
                temp.Scale(Scale.x, Scale.y, Scale.z);
                return temp;
            }
        }

        private bool MatrixInvertible => Math.Abs(Matrix.Determinant) > 1e-6f;

        public Vector3 Forward
        {
            get
            {
                if (!MatrixInvertible)
                    return Vector3.forward;

                var inverseColumn = Matrix.Inverse.Column2;
                return new Vector3(inverseColumn.x, inverseColumn.y, inverseColumn.z).Normalized;
            }
        }

        public Vector3 Up
        {
            get
            {
                if (!MatrixInvertible)
                    return Vector3.up;

                var inverseColumn = Matrix.Inverse.Column1;
                return new Vector3(inverseColumn.x, inverseColumn.y, inverseColumn.z).Normalized;
            }
        }

        public Vector3 Right
        {
            get
            {
                if (!MatrixInvertible)
                    return Vector3.right;

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
            this.Position = position;
            rotationEuler = rotation.ToEulerAngles(); // TODO
            this.Scale = scale;
        }

        /// <summary>
        /// Construct a new TransformComponent with the parameters specified.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="rotationEuler">The entity's rotation.</param>
        /// <param name="scale">The entity's scale.</param>
        public TransformComponent(Vector3 position, Vector3 rotationEuler, Vector3 scale)
        {
            this.Position = position;
            this.rotationEuler = rotationEuler; // TODO
            this.Scale = scale;
        }
    }
}
