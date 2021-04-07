#version 450

in vec3 outVertexPos;
in vec2 outTexCoords;
in vec3 outNormal;

uniform sampler2D diffuseTexture;

uniform int tonemapOperator;

out vec4 fragColor;

void main() 
{
    vec3 hdrColor = texture2D(diffuseTexture, outTexCoords).xyz;
    fragColor = vec4(1.0, 0.0, 1.0, 1.0);
}