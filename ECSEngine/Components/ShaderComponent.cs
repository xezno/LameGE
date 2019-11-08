using System;
using System.Collections.Generic;
using System.IO;

using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        uint shaderProgram;

        public ShaderComponent(params Shader[] shaders)
        {
            shaderProgram = Gl.CreateProgram();
            foreach (var shader in shaders)
            {
                Gl.AttachShader(shaderProgram, shader.glShader);
            }
            Gl.LinkProgram(shaderProgram);
        }

        public void UseShader()
        {
            Gl.UseProgram(shaderProgram);
        }

        public void SetVariable(string v, Matrix4x4f matrix)
        {
            Gl.ProgramUniformMatrix4f(shaderProgram, Gl.GetUniformLocation(shaderProgram, v), 1, false, matrix);
        }
    }
}
