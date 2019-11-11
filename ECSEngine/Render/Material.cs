using System.Collections.Generic;
using System.IO;

using OpenGL;

namespace ECSEngine.Render
{
    // TODO: clean this up somehow
    public class Material
    {
        // Base MTL parameters
        public string materialName;

        public ColorRGB24 ambientColor;
        public Texture2D ambientTexture;
        
        public ColorRGB24 diffuseColor;
        public Texture2D diffuseTexture;
        
        public ColorRGB24 specularColor;
        public Texture2D specularTexture;
        
        public float specularExponent;
        public Texture2D specularExponentTexture;
        
        public float transparency;
        public Texture2D transparencyTexture;
        
        public int illuminationModel;
        
        public Texture2D bumpTexture;

        public Texture2D displacementTexture;

        public Texture2D stencilTexture;
        
        // Clara.io PBR extensions
        public float roughness;
        public Texture2D roughnessTexture;

        public float metallic;
        public Texture2D metallicTexture;

        public float sheen;
        public Texture2D sheenTexture;
        
        public float clearcoatThickness;
        public float clearcoatRoughness;
        
        public ColorRGB24 emissiveColor;
        public Texture2D emissiveTexture;

        public float anisotropy;
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
                    var lineSplit = line.Split(' ');
                    var opcode = lineSplit[0];
                    if (line.StartsWith("newmtl"))
                    {
                        if (currentMaterial != null)
                            materials.Add(currentMaterial);
                        currentMaterial = new Material(line.Substring("newmtl".Length));
                    }

                    /* TODO: Should probably go about this another way:
                     * Each 'opcode' has a particular prefix depending on
                     * what it expects, for the most part.
                     * 
                     * - K* -> R, G, B
                     * - map_* -> Texture name / path
                     * - P* -> value
                     * 
                     * Others are pretty easy to work out and follow similar(-ish)
                     * patterns to those existing.
                     */
                    if (opcode.StartsWith("K"))
                    {
                        // R, G, B
                        ColorRGB24 value = new ColorRGB24(
                            (byte)(float.Parse(lineSplit[1]) * 255),
                            (byte)(float.Parse(lineSplit[2]) * 255),
                            (byte)(float.Parse(lineSplit[3]) * 255)
                            );
                        switch (opcode)
                        {
                            case "Ka": // Ambient color
                                currentMaterial.ambientColor = value;
                                break;
                            case "Kd": // Diffuse color
                                currentMaterial.diffuseColor = value;
                                break;
                            case "Ks": // Specular color
                                currentMaterial.specularColor = value;
                                break;
                            case "Ke": // Emissive
                                currentMaterial.emissiveColor = value;
                                break;
                        }

                    }
                    else if (opcode.StartsWith("map_") || opcode == "bump" || opcode == "norm" || opcode == "norm" || opcode == "disp" || opcode == "decal") // todo: regex this
                    {
                        // Texture name / path
                        Texture2D texture2D = new Texture2D(Path.GetDirectoryName(path) + "/" + lineSplit[1]);
                        switch (opcode)
                        {
                            case "map_Ka": // Ambient texture
                                currentMaterial.ambientTexture = texture2D;
                                break;
                            case "map_Kd": // Diffuse texture
                                currentMaterial.diffuseTexture = texture2D;
                                break;
                            case "map_Ks": // Specular texture
                                currentMaterial.specularTexture = texture2D;
                                break;
                            case "map_Ns": // Specular exponent texture
                                currentMaterial.specularExponentTexture = texture2D;
                                break;
                            case "map_d": // Alpha texture
                                currentMaterial.transparencyTexture = texture2D;
                                break;
                            case "map_bump": // Bump texture
                            case "bump": // Ditto
                            case "norm": // Normal map ("bump" / "map_bump")
                                currentMaterial.bumpTexture = texture2D;
                                break;
                            case "disp": // Displacement map
                                currentMaterial.displacementTexture = texture2D;
                                break;
                            case "decal": // Stencil texture
                                currentMaterial.stencilTexture = texture2D;
                                break;
                        }

                    }
                    else if (opcode.StartsWith("P") || opcode.StartsWith("N") || opcode == "d" || opcode == "Td" || opcode == "aniso" || opcode == "anisor") // todo: regex this
                    {
                        // Float value
                        float value = float.Parse(lineSplit[1]);
                        switch (opcode)
                        {
                            case "Pr": // Roughness
                                currentMaterial.roughness = value;
                                break;
                            case "Pm": // Metallic
                                currentMaterial.metallic = value;
                                break;
                            case "Ps": // Sheen
                                currentMaterial.sheen = value;
                                break;
                            case "Pc": // Clearcoat thickness
                                currentMaterial.clearcoatThickness = value;
                                break;
                            case "Pcr": // Clearcoat roughness
                                currentMaterial.clearcoatRoughness = value;
                                break;
                            case "Ns": // Specular exponent
                                currentMaterial.specularExponent = value;
                                break;
                            case "d": // Transparency
                                currentMaterial.transparency = value;
                                break;
                            case "Tr": // Transparency (inverted; equal to 1-d)
                                currentMaterial.transparency = 1 - value;
                                break;
                            case "aniso": // Anisotropy
                                currentMaterial.anisotropy = value;
                                break;
                            case "anisor": // Anisotropy rotation
                                currentMaterial.anisotropyRot = value;
                                break;
                        }
                    }
                    else if (opcode == "illum")
                    {
                        // Illumination model (see https://en.wikipedia.org/wiki/Wavefront_.obj_file)
                        currentMaterial.illuminationModel = int.Parse(lineSplit[1]);
                    }

                    line = streamReader.ReadLine();
                }
                materials.Add(currentMaterial);
            }
            return materials;
        }
    }
}
