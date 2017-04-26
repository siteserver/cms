using System;
using System.Diagnostics;
using System.Text;
using Top.Api.Util;

namespace Top.Api
{
    /// <summary>
    /// 日志打点的简单实现。
    /// </summary>
    public class DefaultTopLogger : ITopLogger
    {
        private static volatile bool isInit = false;
        private static readonly object initLock = new object();

        private static string _filePath = Constants.LOG_FILE_NAME;
        public static string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                isInit = false;
            }
        }

        private string loggerName;
        private bool isDebugEnabled = false;

        public DefaultTopLogger()
            : this(null, false)
        {
        }

        public DefaultTopLogger(string loggerName)
            : this(loggerName, false)
        {
        }

        public DefaultTopLogger(string loggerName, bool isDebugEnabled)
        {
            this.loggerName = loggerName;
            this.isDebugEnabled = isDebugEnabled;

            if (!isInit)
            {
                lock (initLock)
                {
                    if (!isInit)
                    {
                        try
                        {
                            Trace.Listeners.Add(new TextWriterTraceListener(_filePath));
                        }
                        catch 
                        {
                            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                        }
                        Trace.AutoFlush = true;
                        isInit = true;
                    }
                }
            }
        }

        public bool IsDebugEnabled()
        {
            return this.isDebugEnabled;
        }

        public void TraceApiError(string appKey, string apiName, string url, System.Collections.Generic.Dictionary<string, string> parameters, double latency, string errorMessage)
        {
            StringBuilder info = new StringBuilder();
            info.Append(appKey);
            info.Append(Constants.LOG_SPLIT);
            info.Append(apiName);
            info.Append(Constants.LOG_SPLIT);
            info.Append(TopUtils.GetIntranetIp());
            info.Append(Constants.LOG_SPLIT);
            info.Append(System.Environment.OSVersion.VersionString);
            info.Append(Constants.LOG_SPLIT);
            info.Append(latency);
            info.Append(Constants.LOG_SPLIT);
            info.Append(url);
            info.Append(Constants.LOG_SPLIT);
            info.Append(WebUtils.BuildQuery(parameters));
            info.Append(Constants.LOG_SPLIT);
            info.Append(errorMessage);
            this.Error(info.ToString());
        }

        public void Error(string message)
        {
            Trace.WriteLine(message, getLoggerStart("ERROR"));
        }

        public void Error(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args), getLoggerStart("ERROR"));
        }

        public void Warn(string message)
        {
            Trace.WriteLine(message, getLoggerStart("WARN"));
        }

        public void Warn(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args), getLoggerStart("WARN"));
        }

        public void Info(string message)
        {
            Trace.WriteLine(message, getLoggerStart("INFO"));
        }

        public void Info(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args), getLoggerStart("INFO"));
        }

        public void Debug(string message)
        {
            if (isDebugEnabled)
            {
                Trace.WriteLine(message, getLoggerStart("DEBUG"));
            }
        }

        public void Debug(string format, params object[] args)
        {
            if (isDebugEnabled)
            {
                Trace.WriteLine(string.Format(format, args), getLoggerStart("DEBUG"));
            }
        }

        private string getLoggerStart(string level)
        {
            StringBuilder logInfo = new StringBuilder();
            logInfo.Append(DateTime.Now.ToString(Constants.DATE_TIME_MS_FORMAT));
            logInfo.Append(" ").Append(level);
            if (!string.IsNullOrEmpty(loggerName))
            {
                logInfo.Append(" ").Append(loggerName);
            }
            return logInfo.ToString();
        }

        private static volatile ITopLogger _default;
        private static readonly object lazyLock = new object();
        public static ITopLogger GetDefault()
        {
            if (_default == null)
            {
                lock (lazyLock)
                {
                    if (_default == null)
                    {
                        _default = new DefaultTopLogger();
                    }
                }
            }
            return _default;
        }
    }
}
