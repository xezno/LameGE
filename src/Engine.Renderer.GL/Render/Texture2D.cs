using Engine.Utils.DebugUtils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using PixelFormat = OpenGL.PixelFormat;

namespace Engine.Renderer.GL.Render
{
    public class Texture2D
    {
        /// <summary>
        /// OpenGL's reference to the texture.
        /// </summary>
        public readonly uint glTexture;

        /// <summary>
        /// The path to the texture file ("data" if using a data-based constructor).
        /// </summary>
        public readonly string path;

        /// <summary>
        /// The texture's texture unit.
        /// </summary>
        public readonly TextureUnit textureUnit;

        public int width;
        public int height;

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
            glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            using var textureStream = new MemoryStream();
            var image = Image.FromFile(path);
            Logging.Log($"Image format: {image.PixelFormat}");

            var imageFormat = PixelFormat.Bgra;
            if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ||
                image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                imageFormat = PixelFormat.Bgr;

            image.Save(textureStream, ImageFormat.Bmp);

            var textureData = new byte[textureStream.Length];
            textureStream.Read(textureData, 0, (int)textureStream.Length);

            var textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
            Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, image.Width, image.Height - 1/* fixes black pixel row*/, 0, imageFormat, PixelType.UnsignedByte, textureDataPtr);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            width = image.Width;
            height = image.Height;

            image.Dispose();

            Marshal.FreeHGlobal(textureDataPtr);

            Logging.Log($"Texture {path}: Size {width}x{height}, ptr {glTexture}");
        }

        public Texture2D(byte[] textureData, int width, int height, TextureUnit textureUnit = TextureUnit.Texture0)
        {
            this.width = width;
            this.height = height;
            path = "byteData";
            this.textureUnit = textureUnit;
            glTexture = Gl.GenTexture();

            Gl.BindTexture(TextureTarget.Texture2d, glTexture);

            var textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
            Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

            if (width * height * 4 != textureData.Length)
            {
                throw new Exception("uh oh");
            }

            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, textureDataPtr);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

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
            this.width = width;
            this.height = height;
            path = "intPtrData";
            this.textureUnit = textureUnit;
            glTexture = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, glTexture);
            Gl.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            Gl.GenerateMipmap(TextureTarget.Texture2d);
        }

        public Texture2D(ColorRGB24[] textureData, int width, int height) : this(ConvertColorToBinary(textureData), width, height)
        { }

        public Texture2D(ColorRGBA32[] textureData, int width, int height) : this(ConvertColorToBinary(textureData), width, height)
        { }

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

        static byte[] ConvertColorToBinary(ColorRGB24[] textureData)
        {
            var binaryData = new List<byte>();
            foreach (var color in textureData)
            {
                binaryData.Add(color.r);
                binaryData.Add(color.g);
                binaryData.Add(color.b);
                binaryData.Add(255);
            }

            return binaryData.ToArray();
        }

        static byte[] ConvertColorToBinary(ColorRGBA32[] textureData)
        {
            var binaryData = new List<byte>();
            foreach (var color in textureData)
            {
                binaryData.Add(color.r);
                binaryData.Add(color.g);
                binaryData.Add(color.b);
                binaryData.Add(color.a);
            }

            return binaryData.ToArray();
        }
    }
}
