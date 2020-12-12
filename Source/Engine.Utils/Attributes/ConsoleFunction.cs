using System;

namespace Engine.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ConsoleFunction : Attribute
    {
        public string FunctionName { get; set; }

        public ConsoleFunction(string functionName)
        {
            FunctionName = functionName;
        }
    }
}
