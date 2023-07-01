using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;

namespace LysDicordBot.Commands.PrefixCommands
{
    public class AdminCommands : BaseCommandModule
    {
        [Command("create_rule_reaction")]
        public async Task RuleReaction(CommandContext ctx)
        {
            var message = await ctx.Channel.SendMessageAsync("React here to get roles!");

            await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":thumbsup:"));

            ctx.Client.MessageReactionAdded += async (DiscordClient client, MessageReactionAddEventArgs e) =>
            {
                if (e.Message.Id == message.Id)
                {
                    if (e.Emoji.Name == "👍")
                    {
                        var member = await e.Guild.GetMemberAsync(e.User.Id);

                        var role = e.Guild.GetRole(1124686192932290650);

                        await member.GrantRoleAsync(role);
                    }
                }
            };

            ctx.Client.MessageReactionRemoved += async (DiscordClient client, MessageReactionRemoveEventArgs e) =>
            {
                if (e.Message.Id == message.Id)
                {
                    if (e.Emoji.Name == "👍")
                    {
                        var member = await e.Guild.GetMemberAsync(e.User.Id);

                        var role = e.Guild.GetRole(1124686192932290650);

                        await member.RevokeRoleAsync(role);
                    }
                }
            };
        }

    }
}
