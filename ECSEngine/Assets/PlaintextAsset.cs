using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using ECSEngine.Attributes;
using ECSEngine.Math;
using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Assets
{
    public class PlaintextAsset<T>
    {
        public PlaintextAsset(string path)
        {
            LoadAsset(path);
        }

        private float ParseFloat(string str)
        {
            if (float.TryParse(str, out float tmp))
                return tmp;
            Debug.Log($"Could not successfully parse float from string '{str}'.", Debug.DebugSeverity.High);
            return 0;
        }

        private int ParseInt(string str)
        {
            if (int.TryParse(str, out int tmp))
                return tmp;
            Debug.Log($"Could not successfully parse float from string '{str}'.", Debug.DebugSeverity.High);
            return 0;
        }

        private uint ParseUint(string str)
        {
            if (uint.TryParse(str, out uint tmp))
                return tmp;
            Debug.Log($"Could not successfully parse float from string '{str}'.", Debug.DebugSeverity.High);
            return 0;
        }

        private Vector2 ParseVector2(string str, int startIndex = 0)
        {
            string[] strSplit = str.Split(' ');
            float x = ParseFloat(strSplit[startIndex]);
            float y = ParseFloat(strSplit[startIndex + 1]);
            return new Vector2(x, y);
        }

        private Vector3 ParseVector3(string str, int startIndex = 0)
        {
            string[] strSplit = str.Split(' ');
            float x = ParseFloat(strSplit[startIndex]);
            float y = ParseFloat(strSplit[startIndex + 1]);
            float z = ParseFloat(strSplit[startIndex + 2]);
            return new Vector3(x, y, z);
        }

        // TODO: Reduce code repetitiveness
        // TODO: This does not support multiple meshes / materials per file - should probably implement that
        protected virtual void LoadAsset(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var line = streamReader.ReadLine();
                while (line != null)
                {
                    int textureID = 0;
                    var lineSplit = line.Split(' ');
                    var opcode = lineSplit[0];
                    foreach (var field in typeof(T).GetFields())
                    {
                        foreach (var attribute in field.GetCustomAttributes(typeof(TextAssetOpcodeAttribute), false))
                        {
                            if (((TextAssetOpcodeAttribute)attribute).opcodes.Contains(opcode))
                            {
                                if (field.FieldType == typeof(Texture2D))
                                {
                                    Texture2D texture = new Texture2D("Content/" + lineSplit[1], TextureUnit.Texture0 + textureID++);
                                    field.SetValue(this, texture);
                                }
                                else if (field.FieldType == typeof(ColorRGB24))
                                {
                                    float r = ParseFloat(lineSplit[1]);
                                    float g = ParseFloat(lineSplit[2]);
                                    float b = ParseFloat(lineSplit[3]);
                                    ColorRGB24 color = new ColorRGB24((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                                    field.SetValue(this, color);
                                }
                                else if (field.FieldType == typeof(Vector2))
                                {
                                    field.SetValue(this, ParseVector2(line, 1));
                                }
                                else if (field.FieldType == typeof(Vector3))
                                {
                                    field.SetValue(this, ParseVector3(line, 1));
                                }
                                else if (field.FieldType == typeof(float))
                                {
                                    field.SetValue(this, ParseFloat(lineSplit[1]));
                                }
                                else if (field.FieldType == typeof(int))
                                {
                                    field.SetValue(this, ParseInt(lineSplit[1]));
                                }
                                else if (field.FieldType == typeof(List<Vector3>))
                                {
                                    if (field.GetValue(this) == null)
                                        field.SetValue(this, new List<Vector3>());

                                    ((List<Vector3>)field.GetValue(this)).Add(ParseVector3(line, 1));
                                }
                                else if (field.FieldType == typeof(List<Vector2>))
                                {
                                    if (field.GetValue(this) == null)
                                        field.SetValue(this, new List<Vector2>());

                                    ((List<Vector2>)field.GetValue(this)).Add(ParseVector2(line, 1));
                                }
                                else if (field.FieldType == typeof(List<MeshFaceElement>))
                                {
                                    if (field.GetValue(this) == null)
                                        field.SetValue(this, new List<MeshFaceElement>());

                                    for (int index = 0; index < 3; ++index)
                                    {
                                        var elementSplit = lineSplit[index + 1].Split('/');
                                        uint v = ParseUint(elementSplit[0]) - 1;
                                        uint vt = ParseUint(elementSplit[1]) - 1;
                                        uint vn = ParseUint(elementSplit[2]) - 1;
                                        MeshFaceElement meshFaceElement = new MeshFaceElement(v, vt, vn);
                                        ((List<MeshFaceElement>)field.GetValue(this)).Add(meshFaceElement);
                                    }

                                }
                                else
                                {
                                    Debug.Log($"Unknown attribute type {field.FieldType.Name} on opcode {string.Join(" ", ((TextAssetOpcodeAttribute)attribute).opcodes)}.", Debug.DebugSeverity.High);
                                }
                            }
                        }
                    }
                    line = streamReader.ReadLine();
                }
            }
        }
    }
}
