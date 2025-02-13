using System;
using mxtrAutomation.Common.Ioc;

namespace mxtrAutomation.Common.Utils
{
    public static class Logger
    {
        private static Lazy<ILogger> _lazyLogger = new Lazy<ILogger>(() => ServiceLocator.Current.TryGetInstance<ILogger>());

        public static ILogger ActiveLogger { get { return _lazyLogger.Value; } }

        public static void Log(LogLevel logLevel, string message, params object[] formatParams)
        {
            if (ActiveLogger != null)
                ActiveLogger.Log(logLevel, message, formatParams);
        }

        public static void Log(LogLevel logLevel, Exception e, string message)
        {        
            if (ActiveLogger != null)
                ActiveLogger.Log(logLevel, e, message);
        }

        public static void Log(Exception e, string message)
        {
            if (ActiveLogger != null)
                ActiveLogger.Log(e, message);
        }

        public static void Log(Exception e)
        {
            if (ActiveLogger != null)
                ActiveLogger.Log(e);
        }

        public static void Log(string message)
        {
            if (ActiveLogger != null)
                ActiveLogger.Log(message);
        }

        public static void Log(LogLevel logLevel, Exception e)
        {
            if (ActiveLogger != null)
                ActiveLogger.Log(logLevel, e);
        }

        public static void Reset()
        {
            _lazyLogger = new Lazy<ILogger>(() => ServiceLocator.Current.TryGetInstance<ILogger>());
        }
    }
}
