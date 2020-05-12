using System.Collections.Generic;

namespace Engine.Managers.Scripting
{
    public class ScriptItem
    {
        public string Path { get; set; }
        public string EntryPoint { get; set; }
    }

    public class ScriptConfig
    {
        public List<ScriptItem> ScriptList { get; set; }
        public string Name { get; set; }
        public string License { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Version { get; set; }
    }
}
