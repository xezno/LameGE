using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Engine.Renderer.Primitives
{
    public class Cube
    {
        private float[] cubeVertices = new[] {
            -1.0f,  1.0f, 1.0f,
            -1.0f, -1.0f, 1.0f,
             1.0f, -1.0f, 1.0f,
             1.0f, -1.0f, 1.0f,
             1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f, 1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f, 1.0f,
            -1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f, 1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,

             1.0f, -1.0f, 1.0f,
             1.0f, -1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, 1.0f,
             1.0f, -1.0f, 1.0f,

            -1.0f, -1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f, -1.0f, -1.0f,
            -1.0f, -1.0f, -1.0f,

            -1.0f,  1.0f, 1.0f,
             1.0f,  1.0f, 1.0f,
             1.0f,  1.0f, -1.0f,
             1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, -1.0f,
            -1.0f,  1.0f, 1.0f,

            -1.0f, -1.0f, 1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, 1.0f,
             1.0f, -1.0f, 1.0f,
            -1.0f, -1.0f, -1.0f,
             1.0f, -1.0f, -1.0f
        };

        private float[] cubeUvs = new[]
        {
            0.0f, 1.0f,
            0.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f
        };

        public List<Vertex> Vertices { get; set; }

        public List<uint> Indices
        {
            get
            {
                var indexList = new List<uint>();
                for (uint i = 0; i < Vertices.Count; ++i)
                    indexList.Add(i);

                return indexList;
            }
        }

        private uint vao, vbo;

        public Cube()
        {
            SetupVertices();
            SetupMesh();
        }

        private void SetupVertices()
        {
            List<Vertex> vertices = new List<Vertex>();

            for (int i = 0; i < cubeVertices.Length; i += 3)
            {
                var x = cubeVertices[i];
                var y = cubeVertices[i + 1];
                var z = cubeVertices[i + 2];

                // TODO: better uvs
                var u = cubeUvs[(i / 3) % cubeUvs.Length];
                var v = cubeUvs[((i / 3) + 1) % cubeUvs.Length];

                vertices.Add(new Vertex()
                {
                    Position = new Vector3f(x, y, z),
                    TexCoords = new Vector2f(u, v),

                    // TODO:
                    BiTangent = new Vector3f(0, 0, 0),
                    Normal = new Vector3f(0, 0, 0),
                    Tangent = new Vector3f(0, 0, 0),
                });
            }

            Vertices = vertices;
        }

        public void SetupMesh()
        {
            var vertexStructSize = 14 * sizeof(float);

            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            vbo = Gl.GenBuffer();

            var glVertices = new List<float>();
            foreach (var vertex in Vertices)
            {
                glVertices.AddRange(new[] {
                    vertex.Position.x,
                    vertex.Position.y,
                    vertex.Position.z,

                    vertex.Normal.x,
                    vertex.Normal.y,
                    vertex.Normal.z,

                    vertex.Tangent.x,
                    vertex.Tangent.y,
                    vertex.Tangent.z,

                    vertex.BiTangent.x,
                    vertex.BiTangent.y,
                    vertex.BiTangent.z,

                    vertex.TexCoords.x,
                    vertex.TexCoords.y
                });
            }

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glVertices.Count * sizeof(float), glVertices.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(3 * sizeof(float)));

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(6 * sizeof(float)));

            Gl.EnableVertexAttribArray(3);
            Gl.VertexAttribPointer(3, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(9 * sizeof(float)));

            Gl.EnableVertexAttribArray(4);
            Gl.VertexAttribPointer(4, 2, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(12 * sizeof(float)));

            Gl.BindVertexArray(0);
        }

        public void Draw()
        {
            Gl.BindVertexArray(vao);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Count);
            Gl.BindVertexArray(0);
        }
    }
}
