using ECSEngine.Attributes;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ECSEngine.Assets
{
    /// <summary>
    /// A base class for all assets whose format is made up entirely from standard text; examples include the OBJ and MTL file formats.
    /// </summary>
    /// <typeparam name="T">The derived type.</typeparam>
    public class PlaintextAsset<T>
    {
        /// <summary>
        /// A base class for all assets whose format is made up entirely from standard text; examples include the OBJ and MTL file formats.
        /// </summary>
        /// <param name="path">The path to the asset.</param>
        public PlaintextAsset(string path)
        {
            LoadAsset(path);
        }

        /// <summary>
        /// Create a plaintext asset that has not yet been loaded.
        /// </summary>
        public PlaintextAsset()
        { }

        /// <summary>
        /// Parse a string as a float, and output any issues to the console.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Either: 0 if the string was unable to be parsed correctly, or the float parsed from the string.</returns>
        private float ParseFloat(string str)
        {
            if (float.TryParse(str, out var tmp))
                return tmp;
            Debug.Log($"Could not successfully parse float from string '{str}'.", Debug.Severity.High);
            return 0;
        }

        /// <summary>
        /// Parse a string as an integer, and output any issues to the console.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Either: 0 if the string was unable to be parsed correctly, or the integer parsed from the string.</returns>
        private int ParseInt(string str)
        {
            if (int.TryParse(str, out var tmp))
                return tmp;
            Debug.Log($"Could not successfully parse int from string '{str}'.", Debug.Severity.High);
            return 0;
        }

        /// <summary>
        /// Parse a string as an unsigned integer, and output any issues to the console.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Either: 0 if the string was unable to be parsed correctly, or the unsigned integer parsed from the string.</returns>
        private uint ParseUnsignedInt(string str)
        {
            if (uint.TryParse(str, out var tmp))
                return tmp;
            Debug.Log($"Could not successfully parse unsigned int from string '{str}'.", Debug.Severity.High);
            return 0;
        }

        /// <summary>
        /// Parse a string as a Vector2, and output any issues to the console.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="startIndex">The number of words to ignore before parsing the string.</param>
        /// <returns>Either: 0 if the string was unable to be parsed correctly, or the Vector2 parsed from the string.</returns>
        private Vector2 ParseVector2(string str, int startIndex = 0)
        {
            var strSplit = str.Split(' ');
            var x = ParseFloat(strSplit[startIndex]);
            var y = ParseFloat(strSplit[startIndex + 1]);
            return new Vector2(x, y);
        }


        /// <summary>
        /// Parse a string as a Vector3, and output any issues to the console.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="startIndex">The number of words to ignore before parsing the string.</param>
        /// <returns>Either: 0 if the string was unable to be parsed correctly, or the Vector3 parsed from the string.</returns>
        private Vector3 ParseVector3(string str, int startIndex = 0)
        {
            var strSplit = str.Split(' ');
            var x = ParseFloat(strSplit[startIndex]);
            var y = ParseFloat(strSplit[startIndex + 1]);
            var z = ParseFloat(strSplit[startIndex + 2]);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Load the asset.
        /// </summary>
        /// <param name="assetPath">The path to the asset.</param>
        private void LoadAsset(string assetPath)
        {
            // TODO: Reduce code repetitiveness
            // TODO: This does not support multiple meshes / materials per file - should probably implement that
            using var streamReader = new StreamReader(assetPath);
            var line = streamReader.ReadLine();
            while (line != null)
            {
                var textureId = 0;
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
                                var opcodeLength = opcode.Length + 1; // Opcode length + space character
                                var texturePath = line.Remove(0, opcodeLength);
                                if (!Path.IsPathRooted(texturePath))
                                    texturePath = Path.GetDirectoryName(assetPath) + "/" +
                                                texturePath.Replace('\\', '/');
                                var texture = new Texture2D(texturePath, TextureUnit.Texture0 + textureId++);
                                field.SetValue(this, texture);
                            }
                            else if (field.FieldType == typeof(ColorRGB24))
                            {
                                var r = ParseFloat(lineSplit[1]);
                                var g = ParseFloat(lineSplit[2]);
                                var b = ParseFloat(lineSplit[3]);
                                var color = new ColorRGB24((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
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
                                var value = ParseFloat(lineSplit[1]);
                                if (((TextAssetOpcodeAttribute)attribute).invertValue)
                                    value = 1.0f - value;
                                field.SetValue(this, value);
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

                                for (var index = 0; index < 3; ++index)
                                {
                                    var elementSplit = lineSplit[index + 1].Split('/');
                                    var v = ParseUnsignedInt(elementSplit[0]) - 1;
                                    var vt = ParseUnsignedInt(elementSplit[1]) - 1;
                                    var vn = ParseUnsignedInt(elementSplit[2]) - 1;
                                    var meshFaceElement = new MeshFaceElement(v, vt, vn);
                                    ((List<MeshFaceElement>)field.GetValue(this)).Add(meshFaceElement);
                                }

                            }
                            else
                            {
                                Debug.Log($"Unknown attribute type {field.FieldType.Name} on opcode {string.Join(" ", ((TextAssetOpcodeAttribute)attribute).opcodes)}.", Debug.Severity.High);
                            }
                        }
                    }
                }
                line = streamReader.ReadLine();
            }
        }
    }
}
