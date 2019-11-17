#version 450

struct TextureColorPair {
    vec4 pairColor;
    sampler2D pairTexture;
};

struct TextureFloatPair {
    vec4 pairColor;
    sampler2D pairTexture;
};

struct Material {
    TextureColorPair ambient;
    TextureColorPair diffuse;
    TextureColorPair specular;

    TextureFloatPair specularExponent;
    TextureFloatPair transparency;

    int illuminationModel;

    sampler2D bumpTexture;
    sampler2D displacementTexture;
    sampler2D stencilTexture;

    TextureFloatPair roughness;
    TextureFloatPair metallic;
    TextureFloatPair sheen;

    float clearcoatThickness;
    float clearcoatRoughness;
    
    TextureColorPair emissive;
    float anisotropy;
    float anisotropyRot;
};

in vec3 outVertexPos;
in vec2 outUvCoord;

uniform sampler2D diffuseTexture;
uniform vec4 diffuseColor;
uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec4 frag_color;

void main() {
    vec4 diffuseMix = texture(diffuseTexture, outUvCoord) * diffuseColor;
    frag_color = diffuseMix;
}