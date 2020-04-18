using System;

namespace ECSEngine.DebugUtils
{
    public class DebugCommand
    {
        public string name;
        public string description;

        public bool MatchSuggestion(string input) =>
            (name.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
             description.IndexOf(input, StringComparison.CurrentCultureIgnoreCase) >= 0);
    }

    public class DebugCommand<T> : DebugCommand
    {
        private Func<T> getter;
        private Action<T> setter;

        public T value
        {
            get => getter();
            set => setter(value);
        }

        public DebugCommand(string name, string description, Func<T> getter, Action<T> setter)
        {
            this.name = name;
            this.description = description;
            this.setter = setter;
            this.getter = getter;
        }
    }
}
