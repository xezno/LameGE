using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using OpenGL;

namespace ECSEngine.Render
{
    public class Texture2D
    {
        uint glTexture;
        int repeatType = Gl.REPEAT; // TODO: Get from material
        public TextureUnit textureUnit = TextureUnit.Texture0;
        public Texture2D(string path, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            this.textureUnit = textureUnit;
            this.glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            using (MemoryStream textureStream = new MemoryStream())
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(path);
                byte[] textureData;
                Debug.Log($"Image format: {image.PixelFormat}");

                OpenGL.PixelFormat imageFormat = OpenGL.PixelFormat.Bgra;
                if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb || 
                    image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                    imageFormat = OpenGL.PixelFormat.Bgr;
                image.Save(textureStream, ImageFormat.Bmp);

                if (imageFormat == OpenGL.PixelFormat.Bgra)
                {
                    textureData = new byte[textureStream.Length];
                    textureStream.Read(textureData, 0, (int)textureStream.Length);
                }
                else
                {
                    textureData = new byte[textureStream.Length];
                    textureStream.Read(textureData, 0, (int)textureStream.Length);
                }

                IntPtr textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
                Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
                Gl.GenerateMipmap(TextureTarget.Texture2d);

                image.Dispose();

                Marshal.FreeHGlobal(textureDataPtr);
            }
        }

        public void BindTexture()
        {
            Gl.ActiveTexture(textureUnit);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, repeatType);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, repeatType);
            // TODO: Texture filtering
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
        }
    }
}
