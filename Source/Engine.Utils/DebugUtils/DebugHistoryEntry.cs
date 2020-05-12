using System;
using System.Diagnostics;

namespace Engine.Utils.DebugUtils
{
    public class DebugHistoryEntry
    {
        public DateTime timestamp;
        public StackTrace stackTrace;
        public string str;
        public Logging.Severity severity;

        public DebugHistoryEntry(DateTime timestamp, StackTrace stackTrace, string str, Logging.Severity severity)
        {
            this.timestamp = timestamp;
            this.stackTrace = stackTrace;
            this.str = str;
            this.severity = severity;
        }
    }
}
