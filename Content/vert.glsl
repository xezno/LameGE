#version 450

layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inTexCoord;
layout (location = 2) in vec3 inNormal;

out vec3 outVertexPos;
out vec2 outTexCoord;
out vec3 outNormal;

void main() {
    outVertexPos = inVertexPos;
    outTexCoord = inTexCoord;
    outnormal = inNormal;
    gl_Position = vec4(inVertexPos, 1.0);
}