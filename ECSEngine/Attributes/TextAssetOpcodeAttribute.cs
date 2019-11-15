using System;

namespace ECSEngine.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    internal class TextAssetOpcodeAttribute : Attribute
    {
        public string[] opcodes;

        public TextAssetOpcodeAttribute(params string[] opcodes)
        {
            this.opcodes = opcodes;
        }
    }
}
