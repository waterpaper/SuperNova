using System;
using UnityEngine;

namespace Supernova.Utils
{
    public static class Log
    {
        public static void Info(object o)
        {
            Debug.Log(o.ToString());
        }

        public static void Info(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                Debug.Log(format);
                return;
            }
            Debug.LogFormat(format, args);
        }

        public static void Warning(object o)
        {
            Debug.LogWarning(o.ToString());
        }

        public static void Warning(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                Debug.LogWarning(format);
                return;
            }
            Debug.LogWarningFormat(format, args);
        }

        public static void Error(object o)
        {
            Debug.LogError(o.ToString());
        }

        public static void Error(string format, params object[] args)
        {
            if (args.Length == 0)
            {
                Debug.LogError(format);
                return;
            }
            Debug.LogErrorFormat(format, args);
        }

        public static void Error(Exception exception, string message = null)
        {
            if (message != null)
            {
                Debug.LogError($"{message}\n{exception.Message}\n{exception.StackTrace}");
            }
            else
            {
                Debug.LogError($"{exception.Message}\n{exception.StackTrace}");
            }
        }
    }
}