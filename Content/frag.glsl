#version 450

in vec3 outVertexPos;
in vec2 outUvCoord;

uniform sampler2D albedoTexture;
uniform mat4 projMatrix;
uniform mat4 viewMatrix;
uniform mat4 modelMatrix;

out vec4 frag_color;

void main() {
    frag_color = texture(albedoTexture, outUvCoord);
    // frag_color = vec4(outVertexPos.xy - outUvCoord.xy, 0.0, 1.0);
}