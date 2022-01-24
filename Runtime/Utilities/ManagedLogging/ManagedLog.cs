using UnityEngine;

namespace Utilities.ManagedLogging
{
    public class ManagedLog
    {
        public enum LogType
        {
            Default, Verbose, Structure
        }
        
        public static void Log(object obj, Object context = null, LogType type = default)
        {
            Debug.Log(obj, context);
        }
        
        public static void LogError(object obj)
        {
            Debug.Log(obj);
        }
    }
}