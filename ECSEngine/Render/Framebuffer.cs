using System;
using ECSEngine.Components;
using OpenGL;

namespace ECSEngine.Render
{
    public class Framebuffer
    {
        private readonly uint colorTexture, depthTexture, stencilTexture;
        private readonly uint framebufferObject;

        private readonly ShaderComponent shaderComponent;
        private readonly Mesh mesh = Primitives.Plane;

        public Framebuffer()
        {
            shaderComponent = new ShaderComponent(
                new Shader("Content/Shaders/Framebuffer/main.frag", ShaderType.FragmentShader),
                new Shader("Content/Shaders/Framebuffer/main.vert", ShaderType.VertexShader));

            framebufferObject = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);

            colorTexture = CreateTexture(InternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, colorTexture, 0);

            //depthTexture = CreateTexture(InternalFormat.DepthStencil, PixelFormat.DepthStencil, PixelType.Int);
            //Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthTexture, 0);

            depthTexture = CreateTexture(InternalFormat.DepthComponent, PixelFormat.DepthComponent, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthTexture, 0);
        }

        public void PreRender()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);
            // Render everything else
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

        public uint CreateTexture(InternalFormat internalFormat, PixelFormat pixelFormat, PixelType pixelType)
        {
            var texture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, texture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, internalFormat,
                (int)RenderSettings.Default.gameResolutionX, (int)RenderSettings.Default.gameResolutionY, 0,
                pixelFormat, pixelType, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
            return texture;
        }
    }
}
