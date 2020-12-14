using Engine.Renderer.Components;
using Engine.Renderer.Entities;
using OpenGL;
using System;
using System.Collections.Generic;

namespace Engine.Renderer
{
    public class Mesh
    {
        public struct DrawInfo
        {
            public CameraEntity Camera { get; set; }
            public ShaderComponent Shader { get; set; }
            public LightEntity Light { get; set; }
            public (Cubemap, Cubemap, Cubemap) PbrCubemaps { get; set; }
            public Texture BrdfLut { get; set; }
            public Matrix4x4f ModelMatrix { get; set; }
            public Texture HoloTexture { get; set; }

            public Matrix4x4f ViewMatrix { get; set; }
            public Matrix4x4f ProjMatrix { get; set; }

            public DrawInfo(DrawInfo drawInfo)
            {
                Camera = drawInfo.Camera;
                Shader = drawInfo.Shader;
                Light = drawInfo.Light;
                PbrCubemaps = drawInfo.PbrCubemaps;
                BrdfLut = drawInfo.BrdfLut;
                ModelMatrix = drawInfo.ModelMatrix;
                HoloTexture = drawInfo.HoloTexture;
                ViewMatrix = drawInfo.ViewMatrix;
                ProjMatrix = drawInfo.ProjMatrix;
            }
        }

        public List<Vertex> Vertices { get; set; }
        public List<uint> Indices { get; set; }
        public List<Texture> Textures { get; set; }

        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public int TextureCount { get; private set; }

        private Matrix4x4f localModelMatrix;

        private uint vao, vbo, ebo;

        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures, Matrix4x4f oglTransform)
        {
            Vertices = vertices;
            Indices = indices;
            Textures = textures;
            localModelMatrix = oglTransform;

            SetupMesh();
        }

        private void SetupMesh()
        {
            var vertexStructSize = 14 * sizeof(float);

            vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();

            var glVertices = new List<float>();
            foreach (var vertex in Vertices)
            {
                glVertices.AddRange(new[] {
                    vertex.Position.x,
                    vertex.Position.y,
                    vertex.Position.z,

                    vertex.Normal.x,
                    vertex.Normal.y,
                    vertex.Normal.z,

                    vertex.Tangent.x,
                    vertex.Tangent.y,
                    vertex.Tangent.z,

                    vertex.BiTangent.x,
                    vertex.BiTangent.y,
                    vertex.BiTangent.z,

                    vertex.TexCoords.x,
                    vertex.TexCoords.y
                });
            }

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData(BufferTarget.ArrayBuffer, (uint)glVertices.Count * sizeof(float), glVertices.ToArray(), BufferUsage.StaticDraw);

            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)Indices.Count * sizeof(uint), Indices.ToArray(), BufferUsage.StaticDraw);

            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)0);

            Gl.EnableVertexAttribArray(1);
            Gl.VertexAttribPointer(1, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(3 * sizeof(float)));

            Gl.EnableVertexAttribArray(2);
            Gl.VertexAttribPointer(2, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(6 * sizeof(float)));

            Gl.EnableVertexAttribArray(3);
            Gl.VertexAttribPointer(3, 3, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(9 * sizeof(float)));

            Gl.EnableVertexAttribArray(4);
            Gl.VertexAttribPointer(4, 2, VertexAttribType.Float, false, vertexStructSize, (IntPtr)(12 * sizeof(float)));

            Gl.BindVertexArray(0);

            // Buffered indices & vertices to GPU
            // Keep counts, remove CPU copies
            IndexCount = Indices.Count;
            VertexCount = Vertices.Count;

            Indices.Clear();
            Vertices.Clear();

            Indices = null;
            Vertices = null;
        }

        public void Draw(DrawInfo drawInfo)
        {
            Dictionary<string, uint> counts = new Dictionary<string, uint>();
            List<string> expectedTextures = new List<string>() { "texture_diffuse", "texture_emissive", "texture_unknown", "texture_normal" };

            drawInfo.Shader.Use();

            for (int i = 0; i < Textures.Count; ++i)
            {
                var texture = Textures[i];

                Gl.ActiveTexture(TextureUnit.Texture0 + i);
                Gl.BindTexture(TextureTarget.Texture2d, texture.Id);

                string name = texture.Type;

                if (!counts.ContainsKey(name))
                {
                    counts.Add(name, 0);
                }

                string number = (++counts[name]).ToString();

                drawInfo.Shader.SetBool($"material.{name}{number}.exists", true);
                drawInfo.Shader.SetInt($"material.{name}{number}.texture", i);
            }

            foreach (var texture in expectedTextures)
            {
                if (!counts.ContainsKey(texture))
                {
                    drawInfo.Shader.SetBool($"material.{texture}1.exists", false);
                }
            }
            var cameraComponent = drawInfo.Camera.GetComponent<CameraComponent>();
            var lightComponent = drawInfo.Light.GetComponent<LightComponent>();

            drawInfo.Shader.SetMatrix("projectionMatrix", drawInfo.ProjMatrix);
            drawInfo.Shader.SetMatrix("viewMatrix", drawInfo.ViewMatrix);
            drawInfo.Shader.SetMatrix("modelMatrix", drawInfo.ModelMatrix * localModelMatrix);

            drawInfo.Shader.SetVector3d("camPos", drawInfo.Camera.GetComponent<TransformComponent>().Position);
            drawInfo.Shader.SetVector3d("lightPos", drawInfo.Light.GetComponent<TransformComponent>().Position);

            drawInfo.Shader.SetMatrix("lightProjectionMatrix", lightComponent.ProjMatrix);
            drawInfo.Shader.SetMatrix("lightViewMatrix", lightComponent.ViewMatrix);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count);
            Gl.BindTexture(TextureTarget.Texture2d, lightComponent.ShadowMap.DepthMap);
            drawInfo.Shader.SetInt("shadowMap", Textures.Count);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 1);
            Gl.BindTexture(TextureTarget.TextureCubeMap, drawInfo.PbrCubemaps.Item2.Id);
            drawInfo.Shader.SetInt("irradianceMap", Textures.Count + 1);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 2);
            Gl.BindTexture(TextureTarget.Texture2d, drawInfo.BrdfLut.Id);
            drawInfo.Shader.SetInt("brdfLut", Textures.Count + 2);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 3);
            Gl.BindTexture(TextureTarget.TextureCubeMap, drawInfo.PbrCubemaps.Item3.Id);
            drawInfo.Shader.SetInt("prefilterMap", Textures.Count + 3);

            Gl.ActiveTexture(TextureUnit.Texture0 + Textures.Count + 4);
            Gl.BindTexture(TextureTarget.Texture2d, drawInfo.HoloTexture.Id);
            drawInfo.Shader.SetInt("holoMap", Textures.Count + 4);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }

        public void DrawShadows(LightComponent light, ShaderComponent depthShader, Matrix4x4f modelMatrix)
        {
            depthShader.Use();

            depthShader.SetMatrix("projectionMatrix", light.ProjMatrix);
            depthShader.SetMatrix("viewMatrix", light.ViewMatrix);
            depthShader.SetMatrix("modelMatrix", modelMatrix * localModelMatrix);

            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.BindVertexArray(0);
        }
    }
}
