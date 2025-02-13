using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using mxtrAutomation.Common.Codebase;
using mxtrAutomation.Common.Services;
using Ninject;
using mxtrAutomation.Common.Extensions;

namespace mxtrAutomation.Common.Utils
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public abstract class LoggerBase : ILogger
    {
        [Inject]
        public IEmailer Emailer { get; set; }

        [Inject]
        public IEnvironment Runtime { get; set; }

        protected virtual void LogInternal(LogLevel logLevel, string message, Exception e)
        {
            if (Runtime.Environment == EnvironmentKind.Development)
            {
                Debug.WriteLine(message);
            }
            else
            {
                if (logLevel != LogLevel.Error && logLevel != LogLevel.Fatal)
                    return;

                if (Emailer != null)
                {
                    string[] recipients = ConfigManager.AppSettings["LoggerRecipients"].Split(',');

                    Emailer.SendEmail("webmaster@adtrak360.com", "Logger", recipients, "{0} Logged".With(logLevel), message, false);
                    
                }
            }
        }

        protected virtual void AddExtendedInfo(StringBuilder messageBuilder) { }

        protected string BuildLogMessage(LogLevel logLevel, string message, Exception e)
        {
            if (message.IsNullOrEmpty() && e == null)
                return null;

            StringBuilder messageBuilder = new StringBuilder(message);

            try
            {
                messageBuilder.AppendLine("\tMachine Name: " + Environment.MachineName);

                AddExtendedInfo(messageBuilder);
                AddExceptionDetails(messageBuilder, e);
            }
            catch (Exception ex)
            {
                messageBuilder.AppendLine("Error writing a log entry: " + ex);
#if DEBUG
                throw;
#endif
            }

            return messageBuilder.ToString();
        }

        public void Log(LogLevel logLevel, Exception e, string message)
        {
            LogInternal(logLevel, BuildLogMessage(logLevel, message, e), e);
        }
        public void Log(LogLevel logLevel, string message, params object[] formatParams)
        {
            Log(logLevel, null, String.Format(message, formatParams));
        }
        public void Log(Exception e, string message)
        {
            Log(LogLevel.Error, e, message);
        }
        public void Log(Exception e)
        {
            Log(LogLevel.Error, e, null);
        }
        public void Log(string message)
        {
            Log(LogLevel.Error, null, message);
        }
        public void Log(LogLevel logLevel, Exception e)
        {
            Log(logLevel, e, null);
        }

        /// <summary>
        /// Recursively log details about errors and their contents.
        /// </summary>
        protected static void AddExceptionDetails(StringBuilder messageBuilder, Exception e)
        {
            if (e == null)
                return;

            if (messageBuilder.Length > 0)
                messageBuilder.AppendLine();

            messageBuilder.AppendFormat("{0}: {1}", e.GetType(), e.Message);

            if (e is WebException)
            {
                WebException webException = e as WebException;

                WebResponse webResponse = webException.Response;
                HttpWebResponse httpWebResponse = webResponse as HttpWebResponse;

                if (httpWebResponse != null)
                    messageBuilder.AppendFormat("\nHttp status {0} received from {1}", (int)httpWebResponse.StatusCode, httpWebResponse.ResponseUri);
                else if (webResponse != null)
                    messageBuilder.AppendFormat("\nStatus {0} received from {1}", webException.Status, webResponse.ResponseUri);
                else
                    messageBuilder.AppendFormat("\nStatus {0} received but response was blank", webException.Status);                
            }

            if (e.InnerException != null)
                AddExceptionDetails(messageBuilder, e.InnerException);
        }
    }
}
