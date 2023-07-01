using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.Models
{
    public class TableLog
    {
        public DateTime LogTime { get; set; }
        public string CommandName { get; set; }
        public string UserOfCommand { get; set; }


        public TableLog(DateTime LogTime, string CommandName, string UserOfCommand) 
        {
            this.LogTime = LogTime;
            this.CommandName = CommandName;
            this.UserOfCommand = UserOfCommand;
        }

        public override string ToString()
        {
            return $"Command name: /{CommandName}, User: {UserOfCommand}, LogTime: {LogTime}";
        }
    }
}
