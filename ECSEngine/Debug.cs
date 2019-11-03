using System;
using System.Diagnostics;
using System.Reflection;

namespace ECSEngine
{
    public static class Debug
    {
        public static void Log(string str)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            MethodBase method = stackFrames[1].GetMethod();
            Console.WriteLine($"[{DateTime.Now.ToString()}] {method.ReflectedType.Name}, {method.Name}: {str}");
        }
    }
}
