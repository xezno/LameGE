#version 450

layout(location = 0) in vec3 inVertexPos;
layout(location = 1) in vec2 inUvCoord;

uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec3 outVertexPos;
out vec2 outUvCoord;

void main() {
    outVertexPos = inVertexPos;
    outUvCoord = inUvCoord;

    gl_Position = projMatrix * viewMatrix * modelMatrix * vec4(inVertexPos, 1.0);
}