using Engine.Renderer.GL.Components;
using OpenGL;
using System;

namespace Engine.Renderer.GL.Render
{
    public class Framebuffer
    {
        private readonly uint colorTexture, depthTexture, stencilTexture;
        private readonly uint framebufferObject;

        private readonly ShaderComponent shaderComponent;
        private readonly Mesh mesh = Primitives.Plane;

        public Framebuffer(int gameResX, int gameResY)
        {
            shaderComponent = new ShaderComponent(
                new Shader("Content/Shaders/Framebuffer/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Framebuffer/main.vert", Shader.Type.VertexShader));

            framebufferObject = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);

            colorTexture = CreateTexture(gameResX, gameResY, InternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, colorTexture, 0);

            //depthTexture = CreateTexture(InternalFormat.DepthStencil, PixelFormat.DepthStencil, PixelType.Int);
            //Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthTexture, 0);

            depthTexture = CreateTexture(gameResX, gameResY, InternalFormat.DepthComponent, PixelFormat.DepthComponent, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthTexture, 0);
        }

        public void PreRender()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);
        }

        public void Render()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // Draw to screen

            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.vbo);
            Gl.BindTexture(TextureTarget.Texture2d, colorTexture);

            shaderComponent.SetVariable("texture", 0);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.ElementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public uint CreateTexture(int gameResX, int gameResY, InternalFormat internalFormat, PixelFormat pixelFormat, PixelType pixelType)
        {
            var texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat,
                gameResX, gameResY, 0,
                pixelFormat, pixelType, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            return texture;
        }
    }
}
