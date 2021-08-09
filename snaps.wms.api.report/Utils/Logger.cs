using System;
using NLog;

namespace snaps.wms.api.report.Utils
{
    public class Log : IDisposable
    {
        public string delimeter { get; set; }
        private ILogger _logger;
        public Log(string _delimeter = " | ")
        {
            delimeter = _delimeter;
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void Info(string tx, params string[] messages)
        {
            string _message = "";
            for (int i = 0; i < messages.Length; i++)
            {
                _message = string.Concat(_message, delimeter, messages[i]);
            }
            _logger.Info(string.Concat(tx, _message));
            Console.WriteLine(string.Concat("Info:", tx, _message));
        }
        public void Debug(string tx, params string[] messages)
        {
            string _message = "";
            for (int i = 0; i < messages.Length; i++)
            {
                _message = string.Concat(_message, delimeter, messages[i]);
            }
            _logger.Debug(string.Concat(tx, _message));
            Console.WriteLine(string.Concat("Debug:", tx, _message));
        }
        public void Error(string tx, params string[] messages)
        {
            string _message = "";
            for (int i = 0; i < messages.Length; i++)
            {
                _message = string.Concat(_message, delimeter, messages[i]);
            }
            _logger.Error(string.Concat(tx, _message));
            Console.WriteLine(string.Concat("Error:", tx, _message));
        }
        public void Warning(string tx, params string[] messages)
        {
            string _message = "";
            for (int i = 0; i < messages.Length; i++)
            {
                _message = string.Concat(_message, delimeter, messages[i]);
            }
            _logger.Warn(string.Concat(tx, _message));
            Console.WriteLine(string.Concat("Warning:", tx, _message));
        }

        public void Dispose()
        {
            NLog.LogManager.Shutdown();
        }
    }
}
