using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using OpenGL;

namespace ECSEngine.Render
{
    public class Texture2D
    {
        /// <summary>
        /// OpenGL's reference to the texture.
        /// </summary>
        private readonly uint glTexture;

        /// <summary>
        /// The path to the texture file ("data" if using a data-based constructor).
        /// </summary>
        private string path;

        /// <summary>
        /// The texture's texture unit.
        /// </summary>
        public TextureUnit textureUnit;

        // TODO: Get all below from material
        public int repeatType = Gl.REPEAT;
        public TextureMagFilter magFilter = TextureMagFilter.Nearest;
        public TextureMinFilter minFilter = TextureMinFilter.Nearest;

        /// <summary>
        /// Construct a <see cref="Texture2D"/>, loading the texture from a file.
        /// </summary>
        /// <param name="path">The path to the texture file.</param>
        /// <param name="textureUnit">The texture unit to use.</param>
        public Texture2D(string path, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            this.path = path;
            this.textureUnit = textureUnit;
            this.glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            using MemoryStream textureStream = new MemoryStream();
            System.Drawing.Image image = System.Drawing.Image.FromFile(path);
            Debug.Log($"Image format: {image.PixelFormat}");

            OpenGL.PixelFormat imageFormat = OpenGL.PixelFormat.Bgra;
            if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb || 
                image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                imageFormat = OpenGL.PixelFormat.Bgr;

            image.Save(textureStream, ImageFormat.Bmp);

            var textureData = new byte[textureStream.Length];
            textureStream.Read(textureData, 0, (int)textureStream.Length);

            IntPtr textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
            Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            image.Dispose();

            Marshal.FreeHGlobal(textureDataPtr);
        }

        /// <summary>
        /// Construct a <see cref="Texture2D"/>, loading the texture from a location in memory.
        /// </summary>
        /// <param name="pixels">A pointer to an array of bytes containing an RGBA32 representation of an image.</param>
        /// <param name="width">The texture's width.</param>
        /// <param name="height">The texture's height.</param>
        /// <param name="bpp">The texture's bits per pixel.</param>
        /// <param name="textureUnit">The texture unit to use.</param>
        public Texture2D(IntPtr pixels, int width, int height, int bpp, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            this.path = "data";
            this.textureUnit = textureUnit;
            this.glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            Gl.GenerateMipmap(TextureTarget.Texture2d);
        }

        /// <summary>
        /// Bind the texture to the current OpenGL context.
        /// </summary>
        public void Bind()
        {
            Gl.ActiveTexture(textureUnit);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, repeatType);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, repeatType);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, magFilter);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, minFilter);
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
        }

        public override string ToString()
        {
            return path;
        }
    }
}
