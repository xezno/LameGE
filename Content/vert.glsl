#version 330

layout (location = 0) in vec3 inVertexPos;
layout (location = 1) in vec2 inUvCoord;
layout (location = 2) in vec3 inNormalPos;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec4 outVertexPos;
out vec2 outUvCoord;
out vec3 outNormalPos;

void main() {
    outVertexPos = projMatrix * viewMatrix * modelMatrix * vec4(inVertexPos, 1.0);
    outUvCoord = inUvCoord;
    outNormalPos = inNormalPos;

    gl_Position = outVertexPos;
}