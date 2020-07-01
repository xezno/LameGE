using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine.Utils.DebugUtils
{
    public static class Logging
    {
        public static List<LogEntry> LogEntries = new List<LogEntry>();

        /// <summary>
        /// A shortened copy of <see cref="PastLogs"/> available as a string.
        /// </summary>
        public static string PastLogsString { get; private set; }

        public delegate void DebugLogHandler(LogEntry logEntry);
        public static DebugLogHandler onDebugLog;

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

        private static void WriteLog(StackTrace stackTrace, string logString = "", Severity severity = Severity.Low)
        {
            var logEntry = new LogEntry(DateTime.Now, stackTrace, logString, severity);

            Console.WriteLine(logEntry.ToString()); // TODO: Make engine stable enough that we don't need this anymore

            LogEntries.Add(logEntry);
            onDebugLog?.Invoke(logEntry);
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

            /* BUG: Some functions (namely ogl's debug callback) run on a separate thread, so 
             * they mess with the console's foreground color before another thread has finished outputting.
             * (pls fix) */

            switch (severity)
            {
                case Severity.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case Severity.High:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Severity.Low:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case Severity.Medium:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
            }

            var logTextNoSeverity = str;

            WriteLog(stackTrace, logTextNoSeverity, severity);
        }
    }
}
