using System;
using System.Collections.Generic;
using System.IO;

using ECSEngine.Math;

using OpenGL;

namespace ECSEngine.Render
{
    public class Mesh
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> texCoords = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        List<uint> vertexIndices = new List<uint>();
        List<uint> normalIndices = new List<uint>();
        List<uint> textureIndices = new List<uint>();

        uint VAO, VBO, EBO;

        /// <summary>
        /// Creates a new <see cref="Mesh"/> instance, loading the mesh using <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the desired mesh.</param>
        public Mesh(string path)
        {
            LoadDataAsset(path);
            GenerateBuffers();
        }

        public void GenerateBuffers()
        {
            // Generate VAO, VBO
            Gl.GenVertexArrays(new[] { VAO });
            Gl.GenBuffers(new[] { VBO });
            //GL.GenBuffers(1, out EBO);

            // Buffer data
            uint[] vertexIndices_ = vertexIndices.ToArray();
            float[] vertices_ = new float[vertexIndices_.Length * 8];
            for (int i = 0; i < vertexIndices.Count; ++i)
            {
                vertices_[i * 8 + 7] = normals[(int)normalIndices[i]].x;
                vertices_[i * 8 + 6] = normals[(int)normalIndices[i]].y;
                vertices_[i * 8 + 5] = normals[(int)normalIndices[i]].z;

                vertices_[i * 8 + 4] = texCoords[(int)textureIndices[i]].y;
                vertices_[i * 8 + 3] = texCoords[(int)textureIndices[i]].x;

                vertices_[i * 8 + 2] = vertices[(int)vertexIndices[i]].x;
                vertices_[i * 8 + 1] = vertices[(int)vertexIndices[i]].y;
                vertices_[i * 8] = vertices[(int)vertexIndices[i]].z;
            }

            Gl.BindVertexArray(VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(vertices_.Length * sizeof(float)), vertices_, BufferUsage.StaticDraw);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, vertexIndices_.Length * sizeof(uint), vertexIndices_, BufferUsageHint.StaticDraw);

            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, 8 * sizeof(float), 0 * sizeof(float));
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(2, 3, VertexAttribType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
            Gl.EnableVertexAttribArray(2);
        }

        private static int CountInstancesOfCharInString(string s, char c)
        {
            var i = 0;
            foreach (char c_ in s)
            {
                if (c_ == c)
                {
                    ++i;
                }
            }
            return i;
        }

        public void LoadDataAsset(string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (line.Length < 1 || line[0] == '#') // Comment
                        continue;

                    // Get element id
                    var elementId = line.Remove(line.IndexOf(' '));
                    var parameterCount = CountInstancesOfCharInString(line, ' ');
                    switch (elementId)
                    {
                        case "v": // Vertex position (xyz[w])
                            // Check whether the optional parameter is present or not
                            if (parameterCount == 3)
                            {
                                // Only xyz
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var x = baseLine.Remove(baseLine.IndexOf(' '));
                                var y = baseLine.Remove(0, baseLine.IndexOf(' ') + 1);
                                y = y.Remove(y.LastIndexOf(' ') - 1);
                                var z = baseLine.Remove(0, baseLine.LastIndexOf(' ') + 1);
                                vertices.Add(new Vector3(float.Parse(x), float.Parse(y), float.Parse(z)));
                            }
                            else if (parameterCount == 4)
                            {
                                // xyzw
                                throw new Exception("Optional parameter not implemented yet.");
                            }
                            else
                            {
                                throw new Exception("obj file is not valid.");
                            }
                            break;
                        case "vt": // Texture coordinate (uv[w])
                            if (parameterCount == 2)
                            {
                                // Only uv
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var u = baseLine.Remove(baseLine.IndexOf(' '));
                                var v = baseLine.Remove(0, baseLine.LastIndexOf(' ') + 1);
                                texCoords.Add(new Vector2(float.Parse(u), float.Parse(v)));
                            }
                            else if (parameterCount == 3)
                            {
                                Debug.Log("UVW used");
                                // uvw
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var u = baseLine.Remove(baseLine.IndexOf(' '));
                                var v = baseLine.Remove(0, baseLine.IndexOf(' ') + 1);
                                v = v.Remove(v.LastIndexOf(' ') - 1);
                                var w = baseLine.Remove(0, baseLine.LastIndexOf(' ') + 1);
                                //normals.Add(new Vector3(float.Parse(u), float.Parse(v), float.Parse(w)));
                                texCoords.Add(new Vector2(float.Parse(u), float.Parse(v)));
                            }
                            else
                            {
                                throw new Exception("obj file is not valid.");
                            }
                            break;
                        case "vn": // Vertex normal (xyz)
                            // Check whether the optional parameter is present or not
                            if (parameterCount == 3)
                            {
                                // Only xyz
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var x = baseLine.Remove(baseLine.IndexOf(' '));
                                var y = baseLine.Remove(0, baseLine.IndexOf(' ') + 1);
                                y = y.Remove(y.LastIndexOf(' ') - 1);
                                var z = baseLine.Remove(0, baseLine.LastIndexOf(' ') + 1);
                                normals.Add(new Vector3(float.Parse(x), float.Parse(y), float.Parse(z)));
                            }
                            else
                            {
                                throw new Exception("obj file is not valid.");
                            }
                            break;
                        case "vp": // Parameter space vertex (u[vw])
                            Debug.Log("Parameter space vertices are not supported by this mesh loader.");
                            break;
                        case "f": // Face
                            // Indices
                            if (parameterCount == 3)
                            {
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var tmp = baseLine.Split('/');
                                var parameters = new List<string>();
                                bool[] nVal = new bool[9];
                                int i = 0;
                                foreach (string p in tmp)
                                {
                                    foreach (string s in p.Split(' '))
                                    {
                                        parameters.Add(s);
                                        if (string.IsNullOrEmpty(s))
                                        {
                                            nVal[i] = true;
                                            Debug.Log("Parameter had no value (" + path + ")");
                                        }
                                        i++;
                                    }
                                }
                                vertexIndices.Add((nVal[0] ? 0 : uint.Parse(parameters[0]) - 1)); // v1
                                textureIndices.Add((nVal[1] ? 0 : uint.Parse(parameters[1]) - 1)); // vt1
                                normalIndices.Add((nVal[2] ? 0 : uint.Parse(parameters[2]) - 1)); // vn1

                                vertexIndices.Add((nVal[3] ? 0 : uint.Parse(parameters[3]) - 1)); // v2
                                textureIndices.Add((nVal[4] ? 0 : uint.Parse(parameters[4]) - 1)); // vt2
                                normalIndices.Add((nVal[5] ? 0 : uint.Parse(parameters[5]) - 1)); // vn2

                                vertexIndices.Add((nVal[6] ? 0 : uint.Parse(parameters[6]) - 1)); // v3
                                textureIndices.Add((nVal[7] ? 0 : uint.Parse(parameters[7]) - 1)); // vt3
                                normalIndices.Add((nVal[8] ? 0 : uint.Parse(parameters[8]) - 1)); // vn3
                            }
                            else
                            {
                                Debug.Log("Faces must be triangulated."); // Enable the fucking export option!
                            }
                            break;
                        // TODO below this line
                        case "l": // Line
                            break;
                        case "mtllib": // Define material
                            break;
                        case "usemtl": // Use material
                            break;
                        case "o": // Object
                            break;
                        case "g": // Polygon group
                            break;
                        case "s": // Smooth shading
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
