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

// ========================================
// Cook-Torrance
// (DFG) / (4 * (Wo . n) * (Wi . n))

vec3 lightDirection;
vec3 cameraDirection;
vec3 halfwayDirection;

vec3 normal;
float radiance;

// D = Trowbridge-Reitz GGX
float calcDistribution(float alpha)
{
    float alphaSquared = pow(alpha, 2);
    float nh = max(dot(normal, halfwayDirection), 0.0);
    float nhSquared = pow(nh, 2);

    return (alphaSquared) / (PI * (pow(nhSquared * (alphaSquared - 1.0) + 1.0, 2)));
}

// F = Fresnel-Schlick
vec3 calcFresnel(vec3 f0)
{
    return f0 + (1.0 - f0) * pow(1.0 - radiance, 5.0);
}

// G = Schlick-GGX Smith
float calcGeometric(float k)
{
    float nc = max(dot(normal, cameraDirection), 0.0);
    float nl = max(dot(normal, lightDirection), 0.0);
    float schlickGGX1 = (nc) / (nc * (1.0 - k) + k);
    float schlickGGX2 = (nl) / (nl * (1.0 - k) + k);

    return schlickGGX1 * schlickGGX2;
}

vec3 calcCookTorrance()
{
    vec3 f0 = vec3(0.04);
    f0 = mix(f0, material.diffuseColor.xyz, material.metallic);

    float d = calcDistribution(material.roughness);
    vec3 f = calcFresnel(f0);
    float g = calcGeometric(material.roughness);

    return (d * f * g) / max(4.0 * max(dot(normal, cameraDirection), 0.0) * max(dot(normal, lightDirection), 0.0), 0.001);
}

// ========================================

vec3 getTexture(sampler2D tex)
{
    return texture(tex, outUvCoord).xyz;
}

float calcDiffuseFactor() 
{
    return max(dot(lightDirection, normalize(normal)), 0.0);
}

vec3 calcDiffuseMix()
{
    // TODO: energy conservation
    return calcDiffuseFactor() * (getTexture(material.diffuseTexture) * material.diffuseColor.xyz);
}

vec3 calcAmbientMix()
{
    return material.ambientColor.xyz * 0.05;
}

float calcSpecularFactor()
{
    // Energy conservation (for blinn-phong) = n+8 / 8*pi
    float n = 32.0;
    return ((n + 8)/(8 * PI)) * pow(max(dot(normalize(normal), halfwayDirection), 0.0), n);
}

vec3 calcSpecularMix()
{
    return calcSpecularFactor() * (getTexture(material.specularTexture) * material.specularColor.xyz);
}

float calcAttenuation()
{
    float distance = length(light.pos - cameraPos);
    return 1.0 / (light.constant + light.linear * distance + light.quadratic * pow(distance, 2));
}

void calcAllDirectionVectors()
{
}

vec3 calcFullMix()
{
    vec3 mix = calcAmbientMix() + calcDiffuseMix() + calcSpecularMix();
    mix *= calcCookTorrance();
    return mix;
}

void main() 
{
    lightDirection = normalize(light.pos - modelPos);
    cameraDirection = normalize(cameraPos - modelPos);
    halfwayDirection = normalize(lightDirection + cameraDirection);
    normal = normalize(mat3(transpose(inverse(modelMatrix))) * outNormal);
    radiance = dot(lightDirection, normalize(normal)); // cos(angle)

    modelPos = vec3(modelMatrix * vec4(outFragPos, 1.0));

    calcAllDirectionVectors();

    fragColor = vec4(calcFullMix(), 1.0 - material.transparency);
}