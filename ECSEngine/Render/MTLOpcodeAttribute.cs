using System;

namespace ECSEngine.Render
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    internal class MTLOpcodeAttribute : Attribute
    {
        public string opcode;

        public MTLOpcodeAttribute(string opcode)
        {
            this.opcode = opcode;
        }
    }
}
