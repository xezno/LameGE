using System.IO;

using OpenGL;

namespace ECSEngine.Render
{
    /// <summary>
    /// <see cref="Shader"/> refers to an individual shader that is contained within a shader program (<see cref="Components.ShaderComponent"/>.
    /// </summary>
    public struct Shader
    {
        public string fileName { get; set; }
        public uint glShader { get; set; }
        public ShaderType shaderType { get; set; }

        public Shader(string path, ShaderType shaderType)
        {
            this.shaderType = shaderType;

            fileName = path;
            glShader = Gl.CreateShader(shaderType);

            Compile();

            CheckForErrors();
        }

        private void Compile()
        {
            string[] shaderSource = new string[1];
            using (StreamReader streamReader = new StreamReader(fileName))
                shaderSource[0] = streamReader.ReadToEnd();

            Gl.ShaderSource(glShader, shaderSource);
            Gl.CompileShader(glShader);
        }

        private void CheckForErrors()
        {
            var glErrors = Gl.GetError();
            if (glErrors != ErrorCode.NoError)
            {
                int maxLength = 1024;
                var glErrorStr = new System.Text.StringBuilder(maxLength);
                Gl.GetShaderInfoLog(glShader, maxLength, out int length, glErrorStr);
                Debug.Log($"Problem compiling shader {fileName}: ({length}) {glErrors} - {glErrorStr.ToString()}", Debug.DebugSeverity.High);
            }
            else
            {
                Debug.Log($"Compiled shader {fileName} successfully");
            }
        }
    }
}
