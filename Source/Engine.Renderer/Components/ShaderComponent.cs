using Engine.ECS.Components;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using Newtonsoft.Json;
using OpenGL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Numerics;

namespace Engine.Renderer.Components
{
    public class ShaderComponent : Component<ShaderComponent>
    {
        private List<string> knownMissingVariables = new List<string>();
        public uint Id { get; set; }
        private Asset fragShaderAsset, vertShaderAsset;

        public ShaderComponent(Asset jsonAsset)
        {
            /* 
             * Example:
             *  {
             *      "fragment": "standard.frag",
             *      "vertex": "standard.vert"
             *  }
             */
            var shaderDescription = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonAsset.AsString());
            var directory = Path.GetDirectoryName(jsonAsset.MountPath);
            fragShaderAsset = ServiceLocator.FileSystem.GetAsset($"{directory}/{shaderDescription["fragment"]}");
            vertShaderAsset = ServiceLocator.FileSystem.GetAsset($"{directory}/{shaderDescription["vertex"]}");

            Load();
        }

        public ShaderComponent(Asset fragShaderAsset, Asset vertShaderAsset)
        {
            this.fragShaderAsset = fragShaderAsset;
            this.vertShaderAsset = vertShaderAsset;
            Load();
        }

        public void Load()
        {
            if (ShaderContainer.Shaders.Any(s => s.fragShaderAsset.MountPath == fragShaderAsset.MountPath && s.vertShaderAsset.MountPath == vertShaderAsset.MountPath))
            {
                var cachedShader = ShaderContainer.Shaders.First(s => s.fragShaderAsset.MountPath == fragShaderAsset.MountPath && s.vertShaderAsset.MountPath == vertShaderAsset.MountPath);
                Id = cachedShader.Id;
                return;
            }

            var fragGlslContents = fragShaderAsset.AsString();
            var vertGlslContents = vertShaderAsset.AsString();

            var fragId = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragId, new[] { fragGlslContents });
            Gl.CompileShader(fragId);

            CheckForErrors(fragId);

            var vertId = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertId, new[] { vertGlslContents });
            Gl.CompileShader(vertId);

            CheckForErrors(vertId);

            Id = Gl.CreateProgram();
            Gl.AttachShader(Id, fragId);
            Gl.AttachShader(Id, vertId);
            Gl.LinkProgram(Id);

            Gl.DeleteShader(fragId);
            Gl.DeleteShader(vertId);
        }

        public void Use()
        {
            Gl.UseProgram(Id);
        }

        public void SetFloat(string name, float value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value);
            }
        }

        public void SetInt(string name, int value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value);
            }
        }

        public void SetMatrix(string name, Matrix4x4 value)
        {
            var tmp = new Matrix4x4f(
                value.M11, value.M12, value.M13, value.M14,
                value.M21, value.M22, value.M23, value.M24,
                value.M31, value.M32, value.M33, value.M34,
                value.M41, value.M42, value.M43, value.M44
            );
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniformMatrix4f(Id, loc, 1, false, tmp);
            }
        }

        public void SetVector3(string name, Vector3 value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform3(Id, loc, value.X, value.Y, value.Z);
            }
        }

        public void SetVector2(string name, Vector2 value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform2(Id, loc, (float)value.X, (float)value.Y);
            }
        }

        public void SetBool(string name, bool value)
        {
            if (GetUniformLocation(name, out int loc))
            {
                Gl.ProgramUniform1(Id, loc, value ? 1 : 0);
            }
        }

        private bool GetUniformLocation(string name, out int loc)
        {
            loc = Gl.GetUniformLocation(Id, name);
            if (loc < 0)
            {
                if (knownMissingVariables.Contains(name))
                {
                    return false;
                }

                Logging.Log($"No variable {name}", Logging.Severity.Medium);
                knownMissingVariables.Add(name);
                return false;
            }

            return true;
        }

        private void CheckForErrors(uint shader)
        {
            Gl.GetShader(shader, ShaderParameterName.CompileStatus, out int isCompiled);
            if (isCompiled == 0)
            {
                Gl.GetShader(shader, ShaderParameterName.InfoLogLength, out int maxLength);

                var stringBuilder = new StringBuilder(maxLength);
                Gl.GetShaderInfoLog(shader, maxLength, out int _, stringBuilder);

                Logging.Log(stringBuilder.ToString(), Logging.Severity.High);
            }
        }

        //public override void RenderImGui()
        //{
        //    base.RenderImGui(); var shaderNames = new string[shaders.Length];
        //    for (var i = 0; i < shaders.Length; ++i)
        //    {
        //        shaderNames[i] = shaders[i].FileName;
        //    }

        //    if (shaders.Length > 0)
        //    {
        //        if (ImGui.ListBox("", ref currentShaderItem, shaderNames, shaders.Length))
        //        {
        //            currentShaderSource = shaders[currentShaderItem].ShaderSource[0];
        //        }

        //        if (ImGui.Button("Reload shader"))
        //        {
        //            CreateShader();
        //            shaders[currentShaderItem].ReadSourceFromFile();
        //            shaders[currentShaderItem].Compile();
        //            AttachAndLink();
        //        }
        //    }
        //}
    }

    public class ShaderContainer
    {
        public static List<ShaderComponent> Shaders { get; } = new List<ShaderComponent>();
    }
}
