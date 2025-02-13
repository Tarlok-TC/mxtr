using System;

namespace mxtrAutomation.Common.Utils
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, string message, params object[] formatParams);
        void Log(LogLevel logLevel, Exception e, string message);
        void Log(Exception e, string message);
        void Log(Exception e);
        void Log(string message);
        void Log(LogLevel logLevel, Exception e);
    }
}
