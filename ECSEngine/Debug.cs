using ECSEngine.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ECSEngine
{
    public static class Debug
    {
        /// <summary>
        /// All previous logged strings from the current session.
        /// </summary>
        public static List<string> PastLogs { get; private set; } = new List<string>();

        /// <summary>
        /// A shortened copy of <see cref="PastLogs"/> available as a string.
        /// </summary>
        public static string PastLogsString { get; private set; }

        /// <summary>
        /// The number of lines to use for the <see cref="PastLogsString"/> value.
        /// </summary>
        private static int pastLogsStringLength = 5;

        /// <summary>
        /// Should the <see cref="pastLogsStringConsole"/> use a filter?
        /// </summary>
        private static bool pastLogsStringConsoleUseFilter;

        /// <summary>
        /// All previous logged strings from the current session for display within the <see cref="ImGuiManager"/> console.
        /// </summary>
        public static string pastLogsStringConsole; // TODO: replace with property

        /// <summary>
        /// The available severity levels for a debug message.
        /// </summary>
        public enum Severity
        {
            Low,
            Medium,
            High,
            Fatal
        }

        /// <summary>
        /// Display a message to the console.
        /// </summary>
        /// <param name="str">The message to output.</param>
        /// <param name="severity">The severity of the message, determining its color.</param>
        public static void Log(string str, Severity severity = Severity.Low)
        {
            // Prepare method name & method class name
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            var method = stackFrames?[1].GetMethod();

            /* BUG: Some functions (namely ogl's debug callback) run on a separate thread, so 
             * they mess with the console's foreground color before another thread has finished outputting.
             * (pls fix) */

            switch (severity)
            {
                case Severity.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Severity.High:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case Severity.Low:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case Severity.Medium:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
            }

            var logText =
                $"[{DateTime.Now:T}] {method?.ReflectedType?.Name}, {method?.Name} ({severity}): {str}";

            Console.WriteLine(logText);
            PastLogs.Add(logText);

            // We convert to string here for performance reasons (means we aren't potentially doing it multiple times per frame)
            var pastLogsStart = Math.Max(0, PastLogs.Count - pastLogsStringLength);
            var pastLogsCount = Math.Min(PastLogs.Count, pastLogsStringLength);
            var pastLogsArray = PastLogs.ToArray();
            PastLogsString = string.Join("\n", pastLogsArray, pastLogsStart, pastLogsCount);

            if (!pastLogsStringConsoleUseFilter)
            {
                pastLogsStringConsole = string.Join("\n", pastLogsArray, 0, pastLogsArray.Length);
            }
        }

        public static void CalcLogStringByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                pastLogsStringConsoleUseFilter = false;
                return;
            }

            pastLogsStringConsoleUseFilter = true;

            var filteredLogs = new List<string>();

            foreach (var pastLog in PastLogs)
            {
                if (GetFilterMatch(pastLog, filter))
                    filteredLogs.Add(pastLog);
            }

            pastLogsStringConsole = string.Join("\n", filteredLogs.ToArray(), 0, filteredLogs.Count);
        }

        // TODO: use fuzzy search
        private static bool GetFilterMatch(string str, string filter)
        {
            if (filter.Contains(","))
            {
                foreach (var splitFilter in filter.Split(','))
                {
                    if (str.Contains(splitFilter))
                        return true;
                }

                return false;
            }
            else
            {
                return str.Contains(filter);
            }
        }
    }
}
