using System;

namespace Engine.Utils.DebugUtils
{
    public class DebugMethod<T> : DebugMember
    {
        private Func<T> method;

        public DebugMethod(string name, string description, Func<T> method)
        {
            this.name = name;
            this.description = description;
            this.method = method;
        }
    }
}