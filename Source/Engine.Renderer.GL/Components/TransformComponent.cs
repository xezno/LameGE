﻿using Engine.ECS.Components;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using Quaternion = Engine.Utils.MathUtils.Quaternion;

namespace Engine.Renderer.GL.Components
{
    /// <summary>
    /// A container component for the entity's position and rotation.
    /// </summary>
    public class TransformComponent : Component<TransformComponent>
    {
        private Vector3d position;

        /// <summary>
        /// The entity's position.
        /// </summary>
        public Vector3d Position { get => position; set => position = value; }

        // public Quaternion rotation; // TODO
        private Vector3d rotationEuler;

        /// <summary>
        /// The entity's rotation.
        /// </summary>
        public Vector3d RotationEuler
        {
            get => rotationEuler;
            set => rotationEuler = value % 360f;
        }

        private Vector3d scale;

        /// <summary>
        /// The entity's scale.
        /// </summary>
        public Vector3d Scale { get => scale; set => scale = value; }

        public Matrix4x4f Matrix
        {
            get
            {
                // TODO: Convert doubles to float relative to camera instead of relative to world origin
                var temp = Matrix4x4f.Identity;
                temp.Translate((float)Position.x, (float)Position.y, (float)Position.z);
                temp.RotateX((float)rotationEuler.x);
                temp.RotateY((float)rotationEuler.y);
                temp.RotateZ((float)rotationEuler.z);
                temp.Scale((float)Scale.x, (float)Scale.y, (float)Scale.z);
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
        public TransformComponent(Vector3d position, Quaternion rotation, Vector3d scale)
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
        public TransformComponent(Vector3d position, Vector3d rotationEuler, Vector3d scale)
        {
            this.Position = position;
            this.rotationEuler = rotationEuler; // TODO
            this.Scale = scale;
        }
    }
}
