using System;
using System.Collections.Generic;

using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        private uint shaderProgram;
        private List<string> errorLog; // Prevent console error spam

        public ShaderComponent(params Shader[] shaders)
        {
            shaderProgram = Gl.CreateProgram();
            errorLog = new List<string>();
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

        public void SetVariable(string variableName, object variableValue)
        {
            var variableLocation = Gl.GetUniformLocation(shaderProgram, variableName);
            if (variableLocation < 0 && !errorLog.Contains(variableName))
            {
                errorLog.Add(variableName);
                Debug.Log($"The variable {variableName} does not exist on this shader.", Debug.DebugSeverity.High);
                return;
            }

            // TODO: There's probably a way nicer way of doing this, but I can't think of it right now
            if (variableValue is int)
            {
                Gl.ProgramUniform1(shaderProgram, variableLocation, Convert.ToInt32(variableValue));
            }
            else if (variableValue is float)
            {
                Gl.ProgramUniform1(shaderProgram, variableLocation, Convert.ToSingle(variableValue));
            }
            else if (variableValue is Matrix4x4f)
            {
                var matrix = (Matrix4x4f)variableValue;
                Gl.ProgramUniformMatrix4f(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), 1, false, matrix);
            }
            else if (variableValue is ColorRGB24)
            {
                var color = (ColorRGB24)variableValue;
                Gl.ProgramUniform4(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), color.r / 255f, color.g / 255f, color.b / 255f, 1f);
            }
            else if (variableValue is Texture2D)
            {
                var texture = (Texture2D)variableValue;
                // Determining texture unit # by the enum is just
                // textureUnit - (first texture unit #)
                Gl.ProgramUniform1(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), texture.textureUnit - TextureUnit.Texture0);
            }
            else if (!errorLog.Contains(variableName))
            {
                errorLog.Add(variableName);
                Debug.Log($"I don't know how to handle {variableValue.GetType().Name} yet :(", Debug.DebugSeverity.Medium);
            }
        }
    }
}
