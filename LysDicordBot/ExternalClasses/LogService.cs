using LysDicordBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.ExternalClasses
{
    public class LogService : ILogService
    {
        public string LogPath { get; }
        public LogService(string _logFilePath)
        {
            LogPath = _logFilePath;
        }

        public string Read()
        {
            if (!File.Exists(LogPath)) throw new InvalidDataException("File not found");

            string actual;
            using (var file = new StreamReader(LogPath))
            {
                actual = file.ReadToEnd();
            }
            return actual;
        }

        public void Write(string logInfo)
        {
            File.AppendAllText(LogPath, logInfo + "\n");
        }
    }
}
