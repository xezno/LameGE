#version 450

in vec3 outVertexPos;
in vec2 outTexCoords;
in vec3 outNormal;

uniform sampler2D diffuseTexture;
uniform float exposure;

out vec4 fragColor;
const float gamma = 2.2;

void main() 
{
    vec3 hdrColor = texture2D(diffuseTexture, outTexCoords).xyz;
    vec3 tonemappedColor = vec3(1.0) - exp(-hdrColor * exposure);
    fragColor = vec4(tonemappedColor, 1.0);
}