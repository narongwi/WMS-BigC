using System;
using NLog;

namespace Snaps.Helpers.Logger
{
    public class SnapsLogger : ISnapsLogger, IDisposable
    {
        private string _delimeter { get; set; }
        private ILogger _logger;
        public SnapsLogger(Type type)
        {
            if (type == null)
            {
                _logger = LogManager.GetCurrentClassLogger();
            }
            else
            {
                _logger = LogManager.GetLogger(type.FullName);
            }

            _delimeter = ",";
        }
        public void Info(string taxid, string taxcode, string taxkey, params string[] messages)
        {
            Write(LogLevel.Info, taxid, taxcode, taxkey, null, messages);
        }
        public void Debug(string taxid, string taxcode, string taxkey, params string[] messages)
        {
            if (_logger.IsDebugEnabled)
                Write(LogLevel.Debug, taxid, taxcode, taxkey, null, messages);
        }
        public void Error(string taxid, string taxcode, string taxkey, params string[] messages)
        {
            Write(LogLevel.Error, taxid, taxcode, taxkey, null, messages);
        }
        public void Error(string taxid, string taxcode, string taxkey, Exception ex, params string[] messages)
        {
            Write(LogLevel.Error, taxid, taxcode, taxkey, ex, messages);
        }
        public void Warning(string taxid, string taxcode, string taxkey, params string[] messages)
        {
            if (_logger.IsWarnEnabled)
                Write(LogLevel.Warn, taxid, taxcode, taxkey, null, messages);
        }
        public void Dispose()
        {
            NLog.LogManager.Shutdown();
        }

        public void Write(LogLevel logLevel, string taxid, string taxcode, string taxkey, Exception ex, params string[] messages)
        {

            string _message = "";
            for (int i = 0; i < messages.Length; i++)
                _message = string.Concat(_message, messages[i], i > 0 ? _delimeter : string.Empty);

            LogEventInfo _logevt = new LogEventInfo(logLevel, _logger.Name, _message);
            _logevt.Properties["taxid"] = taxid;
            _logevt.Properties["taxcode"] = taxcode;
            _logevt.Properties["taxkey"] = taxkey;
            _logevt.Exception = ex;
            _logger.Log(typeof(SnapsLogger), _logevt);
            Console.WriteLine(string.Concat(logLevel.ToString(), taxid, _message));
        }
    }

    public class SnapsLogFactory : ISnapsLogFactory
    {
        public ISnapsLogger Create<T>() where T : class
        {
            return new SnapsLogger(typeof(T));
        }

    }
}
