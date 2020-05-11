using System.Collections.Generic;

namespace Engine.Utils.DebugUtils
{
    public class DebugResult
    {
        public DebugResultType Type { get; set; }
        public Dictionary<string, string> Values { get; set; }

        public DebugResult(DebugResultType type, Dictionary<string, string> values)
        {
            Type = type;
            Values = values;
        }
    }
}
