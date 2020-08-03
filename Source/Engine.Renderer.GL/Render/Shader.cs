using Engine.Utils.DebugUtils;
using OpenGL;
using System;
using System.IO;
using System.Text;

namespace Engine.Renderer.GL.Render
{
    /// <summary>
    /// An individual shader that is contained within a <see cref="Components.ShaderComponent"/>.
    /// </summary>
    public struct Shader
    {
        /// <summary>
        /// The shader source's file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// OpenGL's reference to the shader.
        /// </summary>
        public uint GlShader { get; set; }

        public enum Type
        {
            FragmentShader,
            VertexShader
        }

        /// <summary>
        /// The shader's type.
        /// </summary>
        public ShaderType ShaderType { get; private set; }

        private string[] shaderSource;

        /// <summary>
        /// The shader's source.
        /// </summary>
        public string[] ShaderSource
        {
            get => shaderSource;
            set
            {
                shaderSource = value;
                Compile();
            }
        }

        /// <summary>
        /// Construct an instance of <see cref="Shader"/>, compile the shader and check for any errors.
        /// </summary>
        /// <param name="path">The shader source's file name.</param>
        /// <param name="shaderType">The shader's type.</param>
        public Shader(string path, Type shaderType)
        {
            ShaderType = TypeToGlShaderType(shaderType);
            FileName = path;
            GlShader = Gl.CreateShader(ShaderType);

            shaderSource = new string[0];

            ReadSourceFromFile();
            Compile();
        }

        public void Delete()
        {
            Gl.DeleteShader(GlShader);
        }

        public void ReadSourceFromFile()
        {
            shaderSource = new string[1];
            using (var streamReader = new StreamReader(FileName))
                shaderSource[0] = streamReader.ReadToEnd();
        }

        /// <summary>
        /// Read the shader source, and then compile it.
        /// </summary>
        public void Compile()
        {
            Gl.ShaderSource(GlShader, ShaderSource);
            Gl.CompileShader(GlShader);

            CheckForErrors();
        }

        /// <summary>
        /// Check to ensure that the shader compilation was successful; display any errors otherwise.
        /// </summary>
        private void CheckForErrors()
        {
            Gl.GetShader(GlShader, ShaderParameterName.CompileStatus, out int isCompiledInt);
            var isCompiled = (isCompiledInt == 1);

            if (isCompiled)
            {
                Logging.Log($"Compiled shader {FileName} successfully as {GlShader}");
            }
            else
            {
                var maxLength = 1024;
                var glErrorStr = new StringBuilder(maxLength);
                Gl.GetShaderInfoLog(GlShader, maxLength, out var length, glErrorStr);
                Logging.Log($"Problem compiling shader {FileName}: {glErrorStr}", Logging.Severity.High);
            }
        }

        private static ShaderType TypeToGlShaderType(Type type)
        {
            switch (type)
            {
                case Type.VertexShader:
                    return OpenGL.ShaderType.VertexShader;
                case Type.FragmentShader:
                    return OpenGL.ShaderType.FragmentShader;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
