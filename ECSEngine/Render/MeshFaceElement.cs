namespace ECSEngine.Render
{
    public struct MeshFaceElement
    {
        public uint vertexIndex;
        public uint uvIndex;
        public uint normalIndex;

        // f v1/vt1/vn1...
        public MeshFaceElement(uint vertexIndex, uint uvIndex, uint normalIndex)
        {
            this.vertexIndex = vertexIndex;
            this.uvIndex = uvIndex;
            this.normalIndex = normalIndex;
        }
    }
}