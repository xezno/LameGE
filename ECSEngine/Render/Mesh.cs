using System;
using System.Collections.Generic;
using System.IO;

using ECSEngine.Math;

using OpenGL;

namespace ECSEngine.Render
{
    // TODO: Refactor this so that it's a lot more straightforward and maintainable.
    public class Mesh
    {
        public uint VAO, VBO, EBO; // TODO: Make this more abstract so that we can use other APIs / pipelines in the future

        // why
        public int indexCount => vertIndices.Count;
        public int vertexCount => vertices.Count;
        public int normalCount => normals.Count;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvCoords = new List<Vector2>();

        float[] glData;
        uint[] glIndices;

        List<uint> vertIndices = new List<uint>();
        List<uint> normalIndices = new List<uint>();
        List<uint> uvIndices = new List<uint>();

        /// <summary>
        /// Creates a new <see cref="Mesh"/> instance, loading the mesh using <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to the desired mesh.</param>
        public Mesh(string path)
        {
            // "LoadDataAsset" makes no sense as a function name here
            LoadDataAsset(path);

            // generatebuffers should probably be called by the above function
            GenerateBuffers();
        }

        public void GenerateBuffers()
        {
            // Gen objects
            VAO = Gl.GenVertexArray();
            VBO = Gl.GenBuffer();
            EBO = Gl.GenBuffer();

            // Unpack data so that OpenGL can read it
            int vertexAttribSize = 5;
            glIndices = vertIndices.ToArray();
            glData = new float[vertIndices.Count * vertexAttribSize];

            for (int i = 0; i < vertIndices.Count; ++i)
            {
                // TODO: make this simpler
                glData[i * vertexAttribSize + 0] = vertices[(int)vertIndices[i]].x;
                glData[i * vertexAttribSize + 1] = vertices[(int)vertIndices[i]].y;
                glData[i * vertexAttribSize + 2] = vertices[(int)vertIndices[i]].z;
                glData[i * vertexAttribSize + 3] = uvCoords[(int)uvIndices[i]].x;
                glData[i * vertexAttribSize + 4] = uvCoords[(int)uvIndices[i]].y;
            }

            Gl.BindVertexArray(VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glData.Length * sizeof(float), glData, BufferUsage.StaticDraw);

            // Gl.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            // Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)glIndices.Length * sizeof(uint), glIndices, BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexAttribSize * sizeof(float), (IntPtr)0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, vertexAttribSize * sizeof(float), (IntPtr)(3 * sizeof(float)));
        }

        private static int CountInstancesOfCharInString(string s, char c)
        {
            // this is absolutely the most useless function ever
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

        // TODO: Rewrite this
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
                                Debug.Log("Optional vertex parameter not implemented yet.", Debug.DebugSeverity.Medium);
                            }
                            else
                            {
                                Debug.Log("obj file is not valid.", Debug.DebugSeverity.Medium);
                            }
                            break;
                        case "vt": // Texture coordinate (uv[w])
                            if (parameterCount == 2)
                            {
                                // Only uv
                                var baseLine = line.Remove(0, line.IndexOf(' ') + 1);
                                var u = baseLine.Remove(baseLine.IndexOf(' '));
                                var v = baseLine.Remove(0, baseLine.LastIndexOf(' ') + 1);
                                uvCoords.Add(new Vector2(float.Parse(u), float.Parse(v)));
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
                                uvCoords.Add(new Vector2(float.Parse(u), float.Parse(v)));
                            }
                            else
                            {
                                Debug.Log("obj file is not valid.", Debug.DebugSeverity.Medium);
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
                                Debug.Log("obj file is not valid.", Debug.DebugSeverity.Medium);
                            }
                            break;
                        case "vp": // Parameter space vertex (u[vw])
                            Debug.Log("Parameter space vertices are not supported by this mesh loader.", Debug.DebugSeverity.Medium);
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
                                            Debug.Log("Parameter had no value (" + path + ")", Debug.DebugSeverity.Medium);
                                        }
                                        i++;
                                    }
                                }

                                // i literally hate looking at these lines of code.  what is "nval"?? what does it relate to??
                                // TODO: rename these
                                vertIndices.Add((nVal[0] ? 0 : uint.Parse(parameters[0]) - 1)); // v1
                                uvIndices.Add((nVal[1] ? 0 : uint.Parse(parameters[1]) - 1)); // vt1
                                normalIndices.Add((nVal[2] ? 0 : uint.Parse(parameters[2]) - 1)); // vn1

                                vertIndices.Add((nVal[3] ? 0 : uint.Parse(parameters[3]) - 1)); // v2
                                uvIndices.Add((nVal[4] ? 0 : uint.Parse(parameters[4]) - 1)); // vt2
                                normalIndices.Add((nVal[5] ? 0 : uint.Parse(parameters[5]) - 1)); // vn2

                                vertIndices.Add((nVal[6] ? 0 : uint.Parse(parameters[6]) - 1)); // v3
                                uvIndices.Add((nVal[7] ? 0 : uint.Parse(parameters[7]) - 1)); // vt3
                                normalIndices.Add((nVal[8] ? 0 : uint.Parse(parameters[8]) - 1)); // vn3
                            }
                            else
                            {
                                /* TODO: Face triangulation can't be *that* hard, right?  Since this entire thing needs rewriting,
                                 * might as well give it a shot. */
                                Debug.Log("Faces must be triangulated.", Debug.DebugSeverity.Medium); // Export as triangulated faces
                            }
                            break;
                        // TODO: everything below this line
                        case "l": // Line
                        case "mtllib": // Define material
                        case "usemtl": // Use material
                        case "o": // Object
                        case "g": // Polygon group
                        case "s": // Smooth shading
                        default:
                            break;
                    }
                }
            }
        }
    }
}
