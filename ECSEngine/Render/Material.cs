using System.Collections.Generic;
using System.IO;

using OpenGL;

namespace ECSEngine.Render
{
    // TODO: clean this up somehow
    public class Material
    {
        // Base MTL parameters
        [MTLOpcode("newmtl")]
        public string materialName;

        [MTLOpcode("Ka")]
        public ColorRGB24 ambientColor;
        [MTLOpcode("map_Ka")]
        public Texture2D ambientTexture;

        [MTLOpcode("Kd")]
        public ColorRGB24 diffuseColor;
        [MTLOpcode("map_Kd")]
        public Texture2D diffuseTexture;

        [MTLOpcode("Ks")]
        public ColorRGB24 specularColor;
        [MTLOpcode("map_Ks")]
        public Texture2D specularTexture;

        [MTLOpcode("Ns")]
        public float specularExponent;
        [MTLOpcode("map_Ns")]
        public Texture2D specularExponentTexture;

        [MTLOpcode("d")]
        public float transparency;
        [MTLOpcode("map_d")]
        public Texture2D transparencyTexture;

        [MTLOpcode("illum")]
        public int illuminationModel;

        [MTLOpcode("map_bump")]
        [MTLOpcode("bump")]
        [MTLOpcode("norm")]
        public Texture2D bumpTexture;

        [MTLOpcode("disp")]
        public Texture2D displacementTexture;

        [MTLOpcode("decal")]
        public Texture2D stencilTexture;

        // Clara.io PBR extensions
        [MTLOpcode("Pr")]
        public float roughness;
        [MTLOpcode("map_Pr")]
        public Texture2D roughnessTexture;

        [MTLOpcode("Pm")]
        public float metallic;
        [MTLOpcode("map_Pm")]
        public Texture2D metallicTexture;

        [MTLOpcode("Ps")]
        public float sheen;
        [MTLOpcode("map_Ps")]
        public Texture2D sheenTexture;

        [MTLOpcode("Pc")]
        public float clearcoatThickness;
        [MTLOpcode("Pcr")]
        public float clearcoatRoughness;

        [MTLOpcode("Ke")]
        public ColorRGB24 emissiveColor;
        [MTLOpcode("map_Ke")]
        public Texture2D emissiveTexture;

        [MTLOpcode("aniso")]
        public float anisotropy;
        [MTLOpcode("anisor")]
        public float anisotropyRot;

        public Material(string materialName)
        {
            this.materialName = materialName;
        }

        public static List<Material> LoadAllFromFile(string path)
        {
            List<Material> materials = new List<Material>();
            using (var streamReader = new StreamReader(path))
            {
                var line = streamReader.ReadLine();
                Material currentMaterial = null;
                while (line != null)
                {
                    int textureID = 0;
                    var lineSplit = line.Split(' ');
                    var opcode = lineSplit[0];
                    foreach (var field in typeof(Material).GetFields())
                    {
                        foreach (var attribute in field.GetCustomAttributes(typeof(MTLOpcodeAttribute), false))
                        {
                            if (((MTLOpcodeAttribute)attribute).opcode == opcode)
                            {
                                if (field.FieldType == typeof(Texture2D))
                                {
                                    Texture2D texture = new Texture2D("Content/" + lineSplit[1], TextureUnit.Texture0 + textureID++);
                                    field.SetValue(currentMaterial, texture);
                                }
                                else if (field.FieldType == typeof(ColorRGB24))
                                {
                                    float.TryParse(lineSplit[1], out float r);
                                    float.TryParse(lineSplit[2], out float g);
                                    float.TryParse(lineSplit[3], out float b);
                                    ColorRGB24 color = new ColorRGB24((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
                                    field.SetValue(currentMaterial, color);
                                }
                                else if (field.FieldType == typeof(float))
                                {
                                    float.TryParse(lineSplit[1], out float value);
                                    field.SetValue(currentMaterial, value);
                                }
                                else if (field.FieldType == typeof(int))
                                {
                                    int.TryParse(lineSplit[1], out int value);
                                    field.SetValue(currentMaterial, value);
                                }
                                else if (field.FieldType == typeof(string) && opcode == "newmtl")
                                {
                                    currentMaterial = new Material(lineSplit[1]);
                                }
                                else
                                {
                                    Debug.Log($"Unknown MTL attribute type {field.FieldType.Name} on opcode {((MTLOpcodeAttribute)attribute).opcode}.", Debug.DebugSeverity.High);
                                }
                            }
                        }
                    }
                    line = streamReader.ReadLine();
                }
                materials.Add(currentMaterial);
            }
            return materials;
        }
    }
}
