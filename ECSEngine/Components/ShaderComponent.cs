using System.Collections.Generic;
using System.IO;

using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        public uint shaderProgram;

        public ShaderComponent(params Shader[] shaders)
        {
            shaderProgram = Gl.CreateProgram();
            foreach (var shader in shaders)
            {
                var glShader = Gl.CreateShader(shader.shaderType);
                List<string> shaderSource = new List<string>();
                using (StreamReader streamReader = new StreamReader(shader.fileName))
                {
                    shaderSource.Add(streamReader.ReadToEnd());
                }

                Gl.ShaderSource(glShader, shaderSource.ToArray());
                Gl.CompileShader(glShader);
                Gl.AttachShader(glShader, shaderProgram);

                var glErrors = Gl.GetError();
                if (glErrors != ErrorCode.NoError)
                {
                    var glErrorStr = new System.Text.StringBuilder(512);
                    Gl.GetShaderInfoLog(glShader, 512, out int length, glErrorStr);
                    Debug.Log($"Problem compiling shader {shader.fileName}: {glErrors} / {glErrorStr.ToString()}");
                }
                Debug.Log($"Compiled shader {shader.fileName}");
            }
        }
    }
}
