namespace Engine.Renderer.GL.Render
{
    /// <summary>
    /// A representation of an OBJ file format's face element.
    /// </summary>
    public struct MeshFaceElement
    {
        /// <summary>
        /// The index of the vertex.
        /// </summary>
        public uint vertexIndex;

        /// <summary>
        /// The index of the texture UV coordinate.
        /// </summary>
        public uint uvIndex;

        /// <summary>
        /// The index of the normal.
        /// </summary>
        public uint normalIndex;

        /// <summary>
        /// Construct a <see cref="MeshFaceElement"/> with all fields filled.
        /// In the OBJ format, this is represented as "f v1/vt1/vn1...".
        /// </summary>
        /// <param name="vertexIndex">The index of the vertex.</param>
        /// <param name="uvIndex">The index of the texture UV coordinate.</param>
        /// <param name="normalIndex">The index of the normal.</param>
        public MeshFaceElement(uint vertexIndex, uint uvIndex, uint normalIndex)
        {
            this.vertexIndex = vertexIndex;
            this.uvIndex = uvIndex;
            this.normalIndex = normalIndex;
        }
    }
}