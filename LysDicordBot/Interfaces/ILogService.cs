using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.Interfaces
{
    public interface ILogService
    {
        string LogPath { get; }
        void Write(string logInfo);
        string Read();
    }
}
