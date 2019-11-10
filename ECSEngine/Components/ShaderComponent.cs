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

        public void SetVariable(string variableName, Matrix4x4f matrix)
        {
            Gl.ProgramUniformMatrix4f(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), 1, false, matrix);
        }

        public void SetVariable(string variableName, int variableValue)
        {
            Gl.ProgramUniform1(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), variableValue);
        }

        public void SetVariable(string variableName, float variableValue)
        {
            Gl.ProgramUniform1f(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), 1, variableValue);
        }
    }
}
