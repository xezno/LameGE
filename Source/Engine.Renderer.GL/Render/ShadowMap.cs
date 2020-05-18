using Engine.Renderer.GL.Components;
using Engine.Utils;
using OpenGL;
using System;

namespace Engine.Renderer.GL.Render
{
    public class ShadowMap
    {
        uint depthMapTexture;
        uint depthMapFbo;
        
        private readonly ShaderComponent shaderComponent;
        private readonly Mesh mesh = Primitives.Plane;

        public ShadowMap(int resX, int resY)
        {
            depthMapFbo = Gl.GenFramebuffer();

            depthMapTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, depthMapTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.DepthComponent, resX, resY, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFbo);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthMapTexture, 0);
            Gl.DrawBuffer(0);
            Gl.ReadBuffer(0);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            shaderComponent = new ShaderComponent(
                new Shader("Content/Shaders/Depth/Debug/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Depth/Debug/main.vert", Shader.Type.VertexShader));
        }

        public void Bind()
        {
            Gl.Viewport(0, 0, GameSettings.ShadowMapX, GameSettings.ShadowMapY);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, depthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);
        }

        public void Unbind()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void BindTexture()
        {
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, depthMapTexture);
        }

        public void Render()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.vbo);

            BindTexture();

            shaderComponent.SetVariable("depthTexture", 1);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.ElementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }
    }
}
