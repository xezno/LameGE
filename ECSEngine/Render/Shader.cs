using OpenGL;
using System.IO;

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
            this.fileName = path;
            this.glShader = Gl.CreateShader(shaderType);
            CompileShader();
        }

        void CompileShader()
        {
            string[] shaderSource = new string[1];
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                shaderSource[0] = streamReader.ReadToEnd();
            }
            Gl.ShaderSource(glShader, shaderSource);
            Gl.CompileShader(glShader);

            var glErrors = Gl.GetError();
            if (glErrors != ErrorCode.NoError)
            {
                var glErrorStr = new System.Text.StringBuilder(512);
                Gl.GetShaderInfoLog(glShader, 512, out int length, glErrorStr);
                Debug.Log($"Problem compiling shader {fileName}: ({length}) {glErrors} - {glErrorStr.ToString()}", Debug.DebugSeverity.High);
            }
            Debug.Log($"Compiled shader {fileName}");
        }
    }
}
