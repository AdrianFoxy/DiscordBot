using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.Commands.PrefixCommands
{
    public class HelpCommands : BaseCommandModule
    {
        [Command("prefix")]
        public async Task Prefix(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Use prefix >");
        }
    }
}
