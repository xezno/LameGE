using Engine.Assets;
using Engine.Attributes;
using OpenGL;

namespace Engine.Render
{
    public class Material : PlaintextAsset<Material>
    {
        // Base MTL parameters
        [TextAssetOpcode("newmtl")]
        public string materialName;

        [TextAssetOpcode("Ka")]
        public ColorRGB24 ambientColor;
        [TextAssetOpcode("map_Ka")]
        public Texture2D ambientTexture;

        [TextAssetOpcode("Kd")]
        public ColorRGB24 diffuseColor;
        [TextAssetOpcode("map_Kd")]
        public Texture2D diffuseTexture;

        [TextAssetOpcode("Ks")]
        public ColorRGB24 specularColor;
        [TextAssetOpcode("map_Ks")]
        public Texture2D specularTexture;

        [TextAssetOpcode("Ns")]
        public float specularExponent;
        [TextAssetOpcode("map_Ns")]
        public Texture2D specularExponentTexture;

        [TextAssetOpcode("d", invertValue = true)]
        public float transparency;
        [TextAssetOpcode("map_d")]
        public Texture2D transparencyTexture;

        [TextAssetOpcode("illum")]
        public int illuminationModel;

        [TextAssetOpcode("map_bump", "bump", "norm")]
        public Texture2D bumpTexture;

        [TextAssetOpcode("disp")]
        public Texture2D displacementTexture;

        [TextAssetOpcode("decal")]
        public Texture2D stencilTexture;

        // Clara.io PBR extensions
        [TextAssetOpcode("Pr")]
        [Range]
        public float roughness;
        [TextAssetOpcode("map_Pr")]
        public Texture2D roughnessTexture;

        [TextAssetOpcode("Pm")]
        [Range]
        public float metallic;
        [TextAssetOpcode("map_Pm")]
        public Texture2D metallicTexture;

        [TextAssetOpcode("Ps")]
        [Range]
        public float sheen;
        [TextAssetOpcode("map_Ps")]
        public Texture2D sheenTexture;

        [TextAssetOpcode("Pc")]
        public float clearcoatThickness;
        [TextAssetOpcode("Pcr")]
        public float clearcoatRoughness;

        [TextAssetOpcode("Ke")]
        public ColorRGB24 emissiveColor;
        [TextAssetOpcode("map_Ke")]
        public Texture2D emissiveTexture;

        [TextAssetOpcode("aniso")]
        public float anisotropy;
        [TextAssetOpcode("anisor")]
        public float anisotropyRot;

        public Material(string path) : base(path) { }
    }
}
