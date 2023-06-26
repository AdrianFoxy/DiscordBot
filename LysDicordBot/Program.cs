using LysDicordBot;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LysDiscordBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
