using System;
using System.Collections.Generic;
using ECSEngine.Math;
using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        /// <summary>
        /// The OpenGL shader program.
        /// </summary>
        private readonly uint shaderProgram;

        /// <summary>
        /// A list of variables that have already thrown errors; used to prevent the console from becoming too densely populated with shader errors.
        /// </summary>
        private readonly List<string> errorLog;

        /// <summary>
        /// Construct a new ShaderComponent, attaching any of the shaders given.
        /// </summary>
        /// <param name="shaders">Shaders to attach to the shader program.</param>
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

        /// <summary>
        /// Use the shader program with the current OpenGL context.
        /// </summary>
        public void UseShader()
        {
            Gl.UseProgram(shaderProgram);
        }

        /// <summary>
        /// Set a variable within the shader program.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="variableValue">The value to give to the variable.</param>
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
            else if (variableValue is Matrix4x4f matrix)
            {
                Gl.ProgramUniformMatrix4f(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), 1, false, matrix);
            }
            else if (variableValue is Vector3 vector)
            {
                Gl.ProgramUniform3(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), vector.x, vector.y, vector.z);
            }
            else if (variableValue is ColorRGB24 color)
            {
                Gl.ProgramUniform4(shaderProgram, Gl.GetUniformLocation(shaderProgram, variableName), color.r / 255f, color.g / 255f, color.b / 255f, 1f);
            }
            else if (variableValue is Texture2D texture)
            {
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
