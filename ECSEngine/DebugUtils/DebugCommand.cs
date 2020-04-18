namespace ECSEngine.DebugUtils
{
    public class DebugCommand
    {
        public string[] aliases;
        public string description;
        public string value;

        public DebugCommand(string[] aliases, string description, string value)
        {
            this.aliases = aliases;
            this.description = description;
            this.value = value;
        }
    }
}
