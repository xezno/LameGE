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
        public TextureUnit textureUnit = TextureUnit.Texture0;
        // TODO: Get all below from material
        public int repeatType = Gl.REPEAT;
        public TextureMagFilter magFilter = TextureMagFilter.Nearest;
        public TextureMinFilter minFilter = TextureMinFilter.Nearest;

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

                textureData = new byte[textureStream.Length];
                textureStream.Read(textureData, 0, (int)textureStream.Length);

                IntPtr textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
                Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
                Gl.GenerateMipmap(TextureTarget.Texture2d);

                image.Dispose();

                Marshal.FreeHGlobal(textureDataPtr);
            }
        }

        // TODO: actually comment this lool
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pixels">Should be RGBA32</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="bpp"></param>
        public Texture2D(IntPtr pixels, int width, int height, int bpp)
        {
            byte[] data = new byte[width * height * (bpp / 8)];
            Marshal.Copy(pixels, data, 0, data.Length);
            this.glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            using (MemoryStream textureStream = new MemoryStream(data))
            {
                byte[] textureData;
                textureData = new byte[textureStream.Length];
                textureStream.Read(textureData, 0, (int)textureStream.Length);

                IntPtr textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
                Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

                Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, textureDataPtr);
                Gl.GenerateMipmap(TextureTarget.Texture2d);

                Marshal.FreeHGlobal(textureDataPtr);
            }
        }

        public void BindTexture()
        {
            Gl.ActiveTexture(textureUnit);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, repeatType);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, repeatType);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, magFilter);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, minFilter);
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
        }
    }
}
