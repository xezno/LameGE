using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine.Render
{
    /// <summary>
    /// <see cref="Shader"/> refers to an individual shader that is contained within a shader program (<see cref="Components.ShaderComponent"/>.
    /// </summary>
    public struct Shader
    {
        public string fileName { get; set; }
        public uint shader { get; set; }
        public ShaderType shaderType { get; set; }

        public Shader(string path, ShaderType shaderType)
        {
            this.shaderType = shaderType;
            this.fileName = path;
            this.shader = 0; // TODO
        }
    }
}
