using Engine.Renderer.GL.Assets;
using Engine.Utils.Attributes;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Engine.Renderer.GL.Render
{
    // TODO: Move from obj/mtl to any format that doesnt suck

    /// <summary>
    /// A mesh that can be drawn on-screen in 3D space.
    /// </summary>
    public class Mesh : PlaintextAsset<Mesh>
    {
        public uint vao, vbo;

        /// <summary>
        /// The number of OpenGL elements to draw.
        /// </summary>
        public int ElementCount => faceElements.Count;

        [TextAssetOpcode("v")]
        public List<Vector3> vertices = new List<Vector3>();

        [TextAssetOpcode("vn")]
        public List<Vector3> normals = new List<Vector3>();

        [TextAssetOpcode("vt")]
        public List<Vector2> uvCoords = new List<Vector2>();

        [TextAssetOpcode("f")]
        public List<MeshFaceElement> faceElements = new List<MeshFaceElement>();

        /// <summary>
        /// Creates a new <see cref="Mesh"/> instance, loading the mesh using <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the desired mesh.</param>
        public Mesh(string path) : base(path)
        {
            GenerateBuffers();
        }

        /// <summary>
        /// Create a new <see cref="Mesh"/> that has not been loaded from the disk.
        /// </summary>
        public Mesh() { }

        /// <summary>
        /// Generate the relevant OpenGL array and buffer objects, preparing them for on-screen drawing.
        /// </summary>
        public void GenerateBuffers()
        {
            float[] glData;
            // Gen objects
            vao = Gl.GenVertexArray();
            vbo = Gl.GenBuffer();

            // Unpack data so that OpenGL can read it
            var vertexAttribSize = 8;
            glData = new float[faceElements.Count * vertexAttribSize];

            for (var i = 0; i < faceElements.Count; ++i)
            {
                var dataToAdd = new[]
                {
                    vertices[(int)faceElements[i].vertexIndex].x,
                    vertices[(int)faceElements[i].vertexIndex].y,
                    vertices[(int)faceElements[i].vertexIndex].z,
                    uvCoords[(int)faceElements[i].uvIndex].x,
                    uvCoords[(int)faceElements[i].uvIndex].y,
                    normals[(int)faceElements[i].normalIndex].x,
                    normals[(int)faceElements[i].normalIndex].y,
                    normals[(int)faceElements[i].normalIndex].z
                };

                for (var dataIndex = 0; dataIndex < dataToAdd.Length; ++dataIndex)
                    glData[i * vertexAttribSize + dataIndex] = dataToAdd[dataIndex];
            }

            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glData.Length * sizeof(float), glData, BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);
            Gl.EnableVertexAttribArray(2);

            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexAttribSize * sizeof(float), (IntPtr)0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, vertexAttribSize * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.VertexAttribPointer(2, 3, VertexAttribType.Float, false, vertexAttribSize * sizeof(float), (IntPtr)(5 * sizeof(float)));
        }
    }
}
