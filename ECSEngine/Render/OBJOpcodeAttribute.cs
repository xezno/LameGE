using System;

namespace ECSEngine.Render
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    internal class OBJOpcodeAttribute : Attribute
    {
        public string opcode;

        public OBJOpcodeAttribute(string opcode)
        {
            this.opcode = opcode;
        }
    }
}
