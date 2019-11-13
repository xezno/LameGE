using System;

namespace ECSEngine.Render
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    internal class MTLOpcodeAttribute : Attribute
    {
        public string[] opcodes;

        public MTLOpcodeAttribute(params string[] opcodes)
        {
            this.opcodes = opcodes;
        }
    }
}
