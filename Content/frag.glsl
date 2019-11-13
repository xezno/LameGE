#version 450

in vec3 outVertexPos;
in vec2 outUvCoord;

uniform sampler2D diffuseTexture;
uniform vec4 diffuseColor;
uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec4 frag_color;

void main() {
    vec4 diffuseMix = texture(diffuseTexture, outUvCoord) * diffuseColor;
    frag_color = diffuseMix;
}