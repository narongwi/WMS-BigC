using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snaps.Helpers.Logger
{
    public interface ISnapsLogger
    {
        void Info(string taxid, string taxcode, string taxkey, params string[] messages);
         void Debug(string taxid, string taxcode, string taxkey, params string[] messages);
         void Error(string taxid, string taxcode, string taxkey, params string[] messages);
         void Error(string taxid, string taxcode, string taxkey, Exception ex, params string[] messages);
         void Warning(string taxid, string taxcode, string taxkey, params string[] messages);
         void Write(LogLevel logLevel, string taxid, string taxcode, string taxkey, Exception ex, params string[] messages);
    }

    public interface ISnapsLogFactory
    {
        ISnapsLogger Create<T>() where T : class;
    }
}
