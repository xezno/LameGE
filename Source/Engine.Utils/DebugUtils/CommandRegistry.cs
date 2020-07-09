using Engine.Utils.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Engine.Utils.DebugUtils
{
    public static class CommandRegistry
    {
        private static List<DebugMember> debugMembers = new List<DebugMember>();

        private static void RegisterVariable<T>(string name, string description, Func<T> getter, Action<T> setter)
        {
            debugMembers.Add(new DebugVariable<T>(name, description, getter, setter));
        }

        private static void RegisterMethod(string name, string description, Action method)
        {
            debugMembers.Add(new DebugMethod<object>(name, description, () =>
            {
                method.Invoke();
                return null;
            }));
        }

        public static DebugResult ExecuteMethod(string methodName)
        {
            var debugCommandMatch = debugMembers.FirstOrDefault(t => t.name == methodName);

            if (debugCommandMatch == null)
            {
                Logging.Log($"No such method {methodName}");
                return new DebugResult(DebugResultStatus.Failure);
            }

            var methodExecResult = new DebugResult(DebugResultStatus.Success);
            return methodExecResult;
        }

        public static void RegisterAllCommands()
        {
            // TODO: Optimize
            foreach (var method in Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods()).Where(m => m.GetCustomAttributes(typeof(ConsoleFunction), false).Length > 0))
            {
                var consoleFunction = method.GetCustomAttribute<ConsoleFunction>();
                RegisterMethod(consoleFunction.FunctionName, "", () => method.Invoke(null, null));
            }
        }

        public static List<DebugMember> GiveSuggestions(string input)
        {
            List<DebugMember> suggestions = new List<DebugMember>();

            suggestions = (List<DebugMember>)debugMembers.Where(t => t.MatchSuggestion(input)).Take(5);

            //int count = 0;
            //foreach (var command in debugMembers)
            //{
            //    if (command.MatchSuggestion(input))
            //    {
            //        suggestions.Add(command);
            //        count++;
            //    }

            //    if (count > 5)
            //        break;
            //}

            return suggestions;
        }

        #region Debug commands

        [ConsoleFunction("Find")]
        public static string Find(string str)
        {
            List<DebugMember> suggestions = new List<DebugMember>();
            suggestions = (List<DebugMember>)debugMembers.Where(t => t.MatchSuggestion(str));
            return JsonConvert.SerializeObject(suggestions);
        }
        #endregion
    }
}
