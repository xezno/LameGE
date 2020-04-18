using System;
using System.Diagnostics;

namespace ECSEngine.DebugUtils
{
    public class DebugHistoryEntry
    {
        /*
         * Timestamp
         * Stack trace
         * String
         * Severity
         */

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
