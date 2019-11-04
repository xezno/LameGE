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
                Debug.Log($"Compiled shader {shader.fileName}");
            }
        }
    }
}
