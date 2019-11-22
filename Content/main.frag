#version 450

#define PI 3.14159265359

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

vec3 halfwayVector;

// Normal Distribution Function (in this case, Trowbridge-Reitz)
float calcNDF() {
    return pow(material.roughness, 2) / (PI * pow(pow(dot(outNormal, outNormal), 2) * (pow(material.roughness, 2) - 1.0) + 1.0, 2));
}

// Fresnel approximation (Schlick)
float calcFresnel()
{
    float f0 = 1.0;
    return f0 + pow((1.0 - f0)*(1.0 - dot(cameraPos, halfwayVector)), 5);
}

// Geometric attenuation
float calcGA()
{
    // Schlick-GGX
    return pow(material.roughness, 2) / 2.0;
}

float cookTorrance() {
    float spec;
    spec = (calcNDF() * calcFresnel() * calcGA()) / (PI * (dot(cameraPos, outNormal) * dot(outNormal, light.pos)));
    return clamp(spec, 1, 0);
}

void setHalfwayVector()
{
    halfwayVector = cameraPos + light.pos / length(cameraPos + light.pos);
}

void main() {
    setHalfwayVector();

    vec4 diffuseMix = texture(material.diffuseTexture, outUvCoord) * material.diffuseColor;
    vec4 specularMix = cookTorrance() * material.specularColor;

    frag_color = diffuseMix + specularMix;
    frag_color.w = 1.0 - material.transparency;
}