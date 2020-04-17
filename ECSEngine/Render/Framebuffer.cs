using System;
using OpenGL;

namespace ECSEngine.Render
{
    public class Framebuffer
    {
        private uint fbTexture;
        private uint fbObject;

        public Framebuffer()
        {
            fbObject = Gl.GenFramebuffer();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbObject);

            fbTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, fbTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba32f,
                          (int)RenderSettings.Default.gameResolutionX, (int)RenderSettings.Default.gameResolutionY, 0,
                          PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameter(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);

            Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, fbTexture, 0);
        }

        public void PreRender()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, fbObject);
            // Render everything else
        }

        public void Render()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // Draw to screen
        }
    }
}
