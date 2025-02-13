using System;
using mxtrAutomation.Common.Extensions;
using log4net;

namespace mxtrAutomation.Common.Utils
{
    public class Log4NetLogger : LoggerBase
    {
        public Log4NetLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        protected override void LogInternal(LogLevel logLevel, string message, Exception e)
        {
            if (message.IsNullOrEmpty() && e == null)
                return;

            ILog log = LogManager.GetLogger("Default");

            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (log.IsDebugEnabled)
                        log.Debug(message, e);
                    break;
                case LogLevel.Info:
                    if (log.IsInfoEnabled)
                        log.Info(message, e);
                    break;
                case LogLevel.Warn:
                    if (log.IsWarnEnabled)
                        log.Warn(message, e);
                    break;
                case LogLevel.Error:
                    if (log.IsErrorEnabled)
                        log.Error(message, e);
                    break;
                default:
                    log.Fatal(message, e);
                    break;
            }

            base.LogInternal(logLevel, message, e);
        }
    }
}
