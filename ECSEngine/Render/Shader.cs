using OpenGL;
using System.IO;

namespace ECSEngine.Render
{
    /// <summary>
    /// An individual shader that is contained within a <see cref="Components.ShaderComponent"/>.
    /// </summary>
    public struct Shader
    {
        /// <summary>
        /// The shader source's file name.
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// OpenGL's reference to the shader.
        /// </summary>
        public uint glShader { get; set; }

        /// <summary>
        /// The shader's type.
        /// </summary>
        public ShaderType shaderType { get; set; }

        /// <summary>
        /// Construct an instance of <see cref="Shader"/>, compile the shader and check for any errors.
        /// </summary>
        /// <param name="path">The shader source's file name.</param>
        /// <param name="shaderType">The shader's type.</param>
        public Shader(string path, ShaderType shaderType)
        {
            this.shaderType = shaderType;
            fileName = path;
            glShader = Gl.CreateShader(shaderType);

            Compile();

            CheckForErrors();
        }

        /// <summary>
        /// Read the shader source, and then compile it.
        /// </summary>
        private void Compile()
        {
            string[] shaderSource = new string[1];
            using (StreamReader streamReader = new StreamReader(fileName))
                shaderSource[0] = streamReader.ReadToEnd();

            Gl.ShaderSource(glShader, shaderSource);
            Gl.CompileShader(glShader);
        }

        /// <summary>
        /// Check to ensure that the shader compilation was successful; display any errors otherwise.
        /// </summary>
        private void CheckForErrors()
        {
            var glErrors = Gl.GetError();
            if (glErrors != ErrorCode.NoError)
            {
                int maxLength = 1024;
                var glErrorStr = new System.Text.StringBuilder(maxLength);
                Gl.GetShaderInfoLog(glShader, maxLength, out int length, glErrorStr);
                Debug.Log($"Problem compiling shader {fileName}: ({length}) {glErrors} - {glErrorStr}", Debug.DebugSeverity.High);
            }
            else
            {
                Debug.Log($"Compiled shader {fileName} successfully");
            }
        }
    }
}
