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
// Cook-Torrance: (DFG) / (4 * (Wo . n) * (Wi . n))

vec3 lightDirection;
vec3 cameraDirection;
vec3 halfwayDirection;

vec3 normal;
float cosTheta;

// D = Trowbridge-Reitz GGX
float calcDistribution()
{
    float roughnessSquared = pow(material.roughness, 2);
    float nh = max(dot(normal, halfwayDirection), 0.0);
    float nhSquared = pow(nh, 2);

    return (roughnessSquared) / (PI * (pow(nhSquared * (roughnessSquared - 1.0) + 1.0, 2)));
}

// F = Fresnel-Schlick
vec3 calcFresnel(vec3 f0)
{
    return f0 + (1.0 - f0) * pow(1.0 - cosTheta, 5.0);
}

float calcSchlickGGX(float val)
{
    float roughnessSquared = pow(material.roughness + 1.0, 2) / 8.0;
    return (val) / (val * (1.0 - roughnessSquared) + roughnessSquared);
}

// G = Schlick-GGX Smith
float calcGeometric()
{
    float nc = max(dot(normal, cameraDirection), 0.0);
    float nl = max(dot(normal, lightDirection), 0.0);
    float schlickGGX1 = calcSchlickGGX(nc);
    float schlickGGX2 = calcSchlickGGX(nl);

    return schlickGGX1 * schlickGGX2;
}

vec3 calcCookTorrance()
{
    vec3 f0 = vec3(0.5);
    f0 = mix(f0, material.diffuseColor.xyz, material.metallic);

    float d = calcDistribution();
    vec3 f = calcFresnel(f0);
    float g = calcGeometric();

    return vec3(d * f * g) / max(4.0 * max(dot(normal, cameraDirection), 0.0) * max(dot(normal, lightDirection), 0.0), 0.001);
}

vec3 calcFullMix()
{
    vec3 diffuse = texture(material.diffuseTexture, outUvCoord).xyz * material.diffuseColor.xyz;
    return diffuse * vec3(calcCookTorrance());
}

void main() 
{
    // lightDirection = normalize(light.pos - modelPos);
    modelPos = vec3(projMatrix * viewMatrix * modelMatrix * vec4(outFragPos, 1.0));
    lightDirection = normalize(modelPos - light.pos);
    cameraDirection = normalize(cameraPos - modelPos);
    halfwayDirection = normalize(lightDirection + cameraDirection);
    normal = normalize(outNormal);
    cosTheta = max(dot(lightDirection, normal), 0.0); // cos(angle)


    fragColor = vec4(calcFullMix(), 1.0 - material.transparency);
}