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
    float constant;
    float linear;
    float quadratic;
};

in vec3 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormal;
in vec3 outFragPos;

uniform Material material;
uniform Light light;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

uniform vec3 cameraPos;

out vec4 fragColor;

vec3 modelPos;
vec3 halfwayDirection;
vec3 lightDirection;
vec3 cameraDirection;

vec3 normal;

float calcDiffuseFactor() 
{
    return max(dot(lightDirection, normalize(normal)), 0.0);
}

vec3 calcDiffuseMix()
{
    // TODO: energy conservation
    return calcDiffuseFactor() * (texture(material.diffuseTexture, outUvCoord).xyz * material.diffuseColor.xyz);
}

vec3 calcAmbientMix()
{
    return material.ambientColor.xyz * 0.05;
}

float calcSpecularFactor()
{
    // Energy conservation (for blinn-phong) = n+8 / 8*pi
    float n = 8.0;
    return ((n + 8)/(8 * PI)) * pow(max(dot(normalize(normal), halfwayDirection), 0.0), n);
}

vec3 calcSpecularMix()
{
    return calcSpecularFactor() * material.specularColor.xyz;
}

float calcAttenuation()
{
    float distance = length(light.pos - cameraPos);
    return 1.0 / (light.constant + light.linear * distance + light.quadratic * pow(distance, 2));
}

void calcAllDirectionVectors()
{
    lightDirection = normalize(light.pos - modelPos);
    cameraDirection = normalize(cameraPos - modelPos);
    halfwayDirection = normalize(lightDirection + cameraDirection);
}

vec3 calcFullMix()
{
    vec3 mix = calcAmbientMix() + calcDiffuseMix() + calcSpecularMix();
    mix *= calcAttenuation();
    return mix;
}

void main() 
{
    normal = normalize(mat3(transpose(inverse(modelMatrix))) * outNormal);
    modelPos = vec3(modelMatrix * vec4(outFragPos, 1.0));

    calcAllDirectionVectors();

    fragColor = vec4(calcFullMix(), 1.0 - material.transparency);
}