using Engine.Renderer.GL.Components;
using Engine.Utils;
using OpenGL;
using System;

namespace Engine.Renderer.GL.Render
{
    public class Framebuffer
    {
        private readonly uint colorTexture, depthTexture;
        private readonly uint framebufferObject;

        private readonly ShaderComponent shaderComponent;
        private readonly Mesh mesh = Primitives.Plane;
        private float exposure = 0.75f;

        public float Exposure { get => exposure; set => exposure = value; }

        public Framebuffer(int resolutionX, int resolutionY)
        {
            shaderComponent = new ShaderComponent(
                new Shader("Content/Shaders/Framebuffer/framebuffer.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Framebuffer/framebuffer.vert", Shader.Type.VertexShader));

            framebufferObject = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);

            colorTexture = CreateTexture(resolutionX, resolutionY, InternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, colorTexture, 0);

            depthTexture = CreateTexture(resolutionX, resolutionY, InternalFormat.DepthStencil, PixelFormat.DepthComponent, PixelType.Float);
            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2d, depthTexture, 0);

            //depthTexture = CreateTexture(resolutionX, resolutionY, InternalFormat.DepthStencil, PixelFormat.DepthStencil, (PixelType)34042 /* GL_UNSIGNED_INT_24_8 */);
            //Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2d, depthTexture, 0);
        }

        public void Bind()
        {
            Gl.Viewport(0, 0, GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferObject);
        }

        public void Unbind()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render()
        {
            // Draw to screen

            shaderComponent.UseShader();

            Gl.BindVertexArray(mesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.vbo);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, colorTexture);
            Gl.ActiveTexture(TextureUnit.Texture1);
            Gl.BindTexture(TextureTarget.Texture2d, depthTexture);

            shaderComponent.SetVariable("colorTexture", 0);
            shaderComponent.SetVariable("depthTexture", 1);
            shaderComponent.SetVariable("exposure", exposure);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.ElementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        private uint CreateTexture(int gameResX, int gameResY, InternalFormat internalFormat, PixelFormat pixelFormat, PixelType pixelType)
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
