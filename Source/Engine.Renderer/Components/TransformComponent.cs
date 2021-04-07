using Engine.ECS.Components;
using Engine.Utils.MathUtils;
using System;
using System.Numerics;

namespace Engine.Renderer.Components
{
    /// <summary>
    /// A container component for the entity's position and rotation.
    /// </summary>
    public class TransformComponent : Component<TransformComponent>
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; }

        public Matrix4x4 Matrix
        {
            get
            {
                var scaleMatrix = Matrix4x4.CreateScale(Scale);
                var rotationMatrix = Matrix4x4.CreateFromQuaternion(Rotation);
                var positionMatrix = Matrix4x4.CreateTranslation(Position);

                return scaleMatrix * rotationMatrix * positionMatrix;
            }
        }

        private bool MatrixInvertible => Math.Abs(Matrix.GetDeterminant()) > 1e-6f; // accounts for float rounding errors

        public Vector3 Forward
        {
            get
            {
                if (!MatrixInvertible)
                    return new Vector3(0, 0, 1);

                Matrix4x4.Invert(Matrix, out var inverseMatrix);

                return Vector3.Normalize(new Vector3(inverseMatrix.M13, inverseMatrix.M23, inverseMatrix.M33));
            }
        }

        public Vector3 Up
        {
            get
            {
                if (!MatrixInvertible)
                    return new Vector3(0, 1, 0);

                Matrix4x4.Invert(Matrix, out var inverseMatrix);

                return Vector3.Normalize(new Vector3(inverseMatrix.M12, inverseMatrix.M22, inverseMatrix.M32));
            }
        }

        public Vector3 Right
        {
            get
            {
                if (!MatrixInvertible)
                    return new Vector3(1, 0, 0);

                Matrix4x4.Invert(Matrix, out var inverseMatrix);

                return Vector3.Normalize(new Vector3(inverseMatrix.M11, inverseMatrix.M21, inverseMatrix.M31));
            }
        }

        /// <summary>
        /// The transform that this transform is a child of within the scene hierarchy.
        /// </summary>
        public TransformComponent ParentTransform { get; set; }

        /// <summary>
        /// Construct a new TransformComponent with the parameters specified.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="rotation">The entity's rotation.</param>
        /// <param name="scale">The entity's scale.</param>
        public TransformComponent(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.Position = position;
            this.Rotation = rotation;
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
            this.Rotation = QuaternionExtensions.CreateFromEulerAngles(rotationEuler);
            this.Scale = scale;
        }
    }
}
