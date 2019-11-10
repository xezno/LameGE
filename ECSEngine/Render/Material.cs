using OpenGL;
using System;
using System.IO;

namespace ECSEngine.Render
{
    public class Material
    {
        // Base MTL parameters
        ColorRGB24 ambientColor;
        Texture2D ambientTexture;

        ColorRGB24 diffuseColor;
        Texture2D diffuseTexture;

        ColorRGB24 specularColor;
        Texture2D specularTexture;

        float specularExponent;
        Texture2D specularExponentTexture;

        float transparency;
        Texture2D transparencyTexture;

        int illuminationModel;

        Texture2D bumpTexture;
        Texture2D displacementTexture;
        Texture2D stencilTexture;

        // Clara.io extensions
        float roughness;
        Texture2D roughnessTexture;
        float metallic;
        Texture2D metallicTexture;
        float sheen;
        Texture2D sheenTexture;

        float clearcoatThickness;
        float clearcoatRoughness;

        ColorRGB24 emissive;
        Texture2D emissiveTexture;

        float anisotropy;
        float anisotropyRot;
        public Material(string materialName)
        {

        }

        public static void LoadAllFromFile(string path)
        {
            using (var streamReader = new StreamReader(path))
            {
                var line = streamReader.ReadLine();
                var currentMaterial = new Material("unnamed");
                while (line != null)
                {
                    var lineSplit = line.Split(' ');
                    if (line.StartsWith("newmtl"))
                    {
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
                    switch (lineSplit[0])
                    {
                        case "Ka": // Ambient color
                        case "Kd": // Diffuse color
                        case "Ks": // Specular color
                        case "Ns": // Specular exponent
                        case "d": // Transparency
                        case "Tr": // Transparency (inverted; equal to 1-d)
                        case "illum": // Illumination model (see https://en.wikipedia.org/wiki/Wavefront_.obj_file)
                        case "map_Ka": // Ambient texture
                        case "map_Kd": // Diffuse texture
                        case "map_Ks": // Specular texture
                        case "map_Ns": // Specular exponent texture
                        case "map_d": // Alpha texture
                        case "map_bump": // Bump texture
                        case "bump": // Ditto
                        case "disp": // Displacement map
                        case "decal": // Stencil texture
                            break;
                        // Clara.io proposed extensions (may be used in the future)
                        case "Pr": // Roughness
                        case "Pm": // Metallic
                        case "Ps": // Sheen
                        case "Pc": // Clearcoat thickness
                        case "Pcr": // Clearcoat roughness
                        case "Ke": // Emissive
                        case "aniso": // Anisotropy
                        case "anisor": // Anisotropy rotation
                        case "norm": // Normal map ("bump" / "map_bump")
                            Debug.Log($"{lineSplit[0]} is not a supported mtl parameter (yet).");
                            break;
                    }

                    line = streamReader.ReadLine();
                }
            }
        }
    }
}
