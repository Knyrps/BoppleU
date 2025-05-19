using System;
using System.Diagnostics;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Bopple.Core.Utilities
{
    public static class LogUtil
    {
        public static void Log(string message)
        {
            if (!CanDebug())
            {
                return;
            }

            Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (!CanDebug())
            {
                return;
            }

            Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!CanDebug())
            {
                return;
            }

            Debug.LogError(message);
        }

        private static bool CanDebug()
        {

            StackTrace trace = new();
            StackFrame frame = trace.GetFrame(2);
            MethodBase method = frame.GetMethod();
            Type declaringType = method.DeclaringType;
            if (declaringType?.GetCustomAttribute<DebugAttribute>() == null && method?.GetCustomAttribute<DebugAttribute>() == null)
            {
                return false;
            }

            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DebugAttribute : Attribute
    {
        public DebugAttribute() { }
    }
}