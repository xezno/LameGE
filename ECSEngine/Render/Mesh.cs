﻿using System;
using System.Collections.Generic;

using ECSEngine.Assets;
using ECSEngine.Attributes;
using ECSEngine.Math;

using OpenGL;

namespace ECSEngine.Render
{
    // TODO: Move from obj/mtl to glTF

    /// <summary>
    /// A mesh that can be drawn on-screen in 3D space.
    /// </summary>
    public class Mesh : PlaintextAsset<Mesh>
    {
        public uint VAO, VBO;

        /// <summary>
        /// The number of OpenGL elements to draw.
        /// </summary>
        public int elementCount => faceElements.Count;

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
        /// Generate the relevant OpenGL array and buffer objects, preparing them for on-screen drawing.
        /// </summary>
        public void GenerateBuffers()
        {
            float[] glData;
            // Gen objects
            VAO = Gl.GenVertexArray();
            VBO = Gl.GenBuffer();

            // Unpack data so that OpenGL can read it
            int vertexAttribSize = 8;
            glData = new float[faceElements.Count * vertexAttribSize];

            for (int i = 0; i < faceElements.Count; ++i)
            {
                // TODO: make this simpler
                glData[i * vertexAttribSize + 0] = vertices[(int)faceElements[i].vertexIndex].x;
                glData[i * vertexAttribSize + 1] = vertices[(int)faceElements[i].vertexIndex].y;
                glData[i * vertexAttribSize + 2] = vertices[(int)faceElements[i].vertexIndex].z;
                glData[i * vertexAttribSize + 3] = uvCoords[(int)faceElements[i].uvIndex].x;
                glData[i * vertexAttribSize + 4] = uvCoords[(int)faceElements[i].uvIndex].y;
                glData[i * vertexAttribSize + 5] = normals[(int)faceElements[i].normalIndex].x;
                glData[i * vertexAttribSize + 6] = normals[(int)faceElements[i].normalIndex].y;
                glData[i * vertexAttribSize + 7] = normals[(int)faceElements[i].normalIndex].z;
            }

            Gl.BindVertexArray(VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, VBO);
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
