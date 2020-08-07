﻿namespace Engine.Utils.MathUtils
{
    // TODO: Consider removing because OpenGL.Net.Math has built-in Vertex2f classes
    public struct Vector2f
    {
        /// <summary>
        /// The point at which the vector resides on the X axis
        /// </summary>
        public float x;

        /// <summary>
        /// The point at which the vector resides on the Y axis
        /// </summary>
        public float y;

        /// <summary>
        /// Construct a <see cref="Vector2f"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        public Vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2f Multiply(Vector2f a, Vector2f b) => new Vector2f(a.x * b.x,
                                                                            a.y * b.y);
        public static Vector2f Subtract(Vector2f a, Vector2f b) => new Vector2f(a.x - b.x,
                                                                            a.y - b.y);
        public static Vector2f Divide(Vector2f a, Vector2f b) => new Vector2f(a.x / b.x,
                                                                            a.y / b.y);
        public static Vector2f Add(Vector2f a, Vector2f b) => new Vector2f(a.x + b.x,
                                                                            a.y + b.y);

        public static Vector2f operator *(Vector2f a, Vector2f b) => Multiply(a, b);
        public static Vector2f operator -(Vector2f a, Vector2f b) => Subtract(a, b);
        public static Vector2f operator /(Vector2f a, Vector2f b) => Divide(a, b);
        public static Vector2f operator +(Vector2f a, Vector2f b) => Add(a, b);

        public static Vector2f operator *(Vector2f a, float b) => new Vector2f(a.x * b,
                                                                            a.y * b);
        public static Vector2f operator -(Vector2f a, float b) => new Vector2f(a.x - b,
                                                                            a.y - b);
        public static Vector2f operator /(Vector2f a, float b) => new Vector2f(a.x / b,
                                                                            a.y / b);
        public static Vector2f operator +(Vector2f a, float b) => new Vector2f(a.x + b,
                                                                            a.y + b);
        /// <summary>
        /// Get all values within the <see cref="Vector2f"/> as a string.
        /// </summary>
        /// <returns>Both coordinates (<see cref="x"/> and <see cref="y"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}";
        }
    }
}