#version 450

struct Material {
    vec4 ambientColor;
    sampler2D ambientTexture;

    vec4 diffuseColor;
    sampler2D diffuseTexture;

    vec4 specularColor;
    sampler2D specularTexture;

    float specularExponentColor;
    sampler2D specularExponentTexture;

    float transparency;
    sampler2D transparencyTexture;

    int illuminationModel;

    sampler2D bumpTexture;
    sampler2D displacementTexture;
    sampler2D stencilTexture;

    float roughness;
    sampler2D roughnessTexture;

    float metallic;
    sampler2D metallicTexture;

    float sheen;
    sampler2D sheenTexture;

    float clearcoatThickness;
    float clearcoatRoughness;
    
    vec4 emissiveColor;
    sampler2D emissiveTexture;

    float anisotropy;
    float anisotropyRot;
};

struct Light {
    vec3 pos;
    float range;
    float dist;
    float quadratic;
};

in vec3 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormal;

uniform Material material;
uniform Light light;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform vec3 cameraPos;

out vec4 frag_color;

float blinnPhong() {
    vec3 halfwayVector = cameraPos + light.pos / length(cameraPos + light.pos);
    return length(outNormal) * length(halfwayVector);
}

void main() {
    vec4 diffuseMix = texture(material.diffuseTexture, outUvCoord) * material.diffuseColor;
    vec4 specularMix = blinnPhong() * material.specularColor;
    frag_color = diffuseMix + specularMix;
    frag_color.w = 1.0 - material.transparency;

    frag_color = vec4(outNormal, 1.0);
}