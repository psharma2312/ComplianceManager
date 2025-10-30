using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ComplianceManager
{
    public class ActionLogger
    {
        private readonly string _username;
        private readonly string _correlationId;
        private readonly string _baseLogPath;
        private string _currentLogFile;
        private DateTime _currentDate;

        public ActionLogger(string username, string correlationId)
        {
            _username = username;
            _correlationId = correlationId;
            _baseLogPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Logs"), _username);
            Directory.CreateDirectory(_baseLogPath);
            _currentDate = DateTime.Now.Date;
            _currentLogFile = GetLogFilePath(_currentDate);
        }

        private string GetLogFilePath(DateTime date)
        {
            string fileName = $"{date:yyyy-MM-dd}.log";
            return Path.Combine(_baseLogPath, fileName);
        }

        private void WriteLog(string level, string message)
        {
            DateTime now = DateTime.Now;
            if (now.Date != _currentDate)
            {
                _currentDate = now.Date;
                _currentLogFile = GetLogFilePath(_currentDate);
            }

            string logEntry = $"{now:yyyy-MM-dd HH:mm:ss} [{level}] [CID:{_correlationId}] - {message}";
            File.AppendAllText(_currentLogFile, logEntry + Environment.NewLine);
        }

        public void Info(string message) => WriteLog("INFO", message);
        public void Debug(string message) => WriteLog("DEBUG", message);
        public void Error(string message) => WriteLog("ERROR", message);
    }
}