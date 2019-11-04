#version 450

in vec3 outVertexPos;
in vec2 outTexCoord;
in vec3 outNormal;

void main() {
    frag_color = vec4(outVertexPos, 1.0);
}