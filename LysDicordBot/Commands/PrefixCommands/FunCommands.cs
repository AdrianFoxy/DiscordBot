using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.EventHandling;

namespace LysDicordBot.Commands.PrefixCommands
{
    public class FunCommands : BaseCommandModule
    {

        [Command("tell_me_the_truth")]
        public async Task Truth(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I'm a better than calculator, I have a soul. I can also send hentai pictures :) And later I will start sending hentai pictures to Maxim's mailbox :))))");
        }

        [Command("add")]
        public async Task Addition(CommandContext ctx, double number1, double number2)
        {
            await ctx.Channel.SendMessageAsync($"Result: {number1 + number2}");
        }

        [Command("difference")]
        public async Task Difference(CommandContext ctx, double number1, double number2)
        {
            await ctx.Channel.SendMessageAsync($"Result: {number1 - number2}");
        }

        [Command("subtract")]
        public async Task Subtract(CommandContext ctx, double number1, double number2)
        {
            await ctx.Channel.SendMessageAsync($"Result: {number1 * number2}");
        }

        [Command("division")]
        public async Task Division(CommandContext ctx, double number1, double number2)
        {
            await ctx.Channel.SendMessageAsync($"Result: {number1 / number2}");
        }
    }
}
