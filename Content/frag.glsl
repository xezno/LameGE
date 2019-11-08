#version 330

in vec4 outVertexPos;
in vec2 outUvCoord;
in vec3 outNormalPos;

out vec4 frag_color;

void main() {
    frag_color = vec4(outVertexPos.xyz, 1.0);
}