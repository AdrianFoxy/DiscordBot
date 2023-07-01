using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LysDicordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LysDicordBot.Commands.SlashCommands
{
    public class HelpCommands : ApplicationCommandModule
    {
        [SlashCommand("help", "help info special for u <3")]
        public async Task HelpCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent("Looking for help..."));
            DiscordButtonComponent pullHelp = new DiscordButtonComponent(ButtonStyle.Success, "1", "pull help");
            DiscordButtonComponent tableHelp = new DiscordButtonComponent(ButtonStyle.Success, "2", "table help");
            DiscordButtonComponent authorHelp = new DiscordButtonComponent(ButtonStyle.Secondary, "3", "bot author info");

            var message = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("Looks like u need some help :thinking: ")
                .WithDescription("Please select a button with info you need:")

                )
                .AddComponents(pullHelp)
                .AddComponents(tableHelp)
                .AddComponents(authorHelp);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Help has arrived!"));
            await ctx.Channel.SendMessageAsync(message);
        }

        [SlashCommand("clear", "delete all bot commands from channel")]
        public async Task Clear(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var botMessages = new List<DiscordMessage>();
            var messages = await ctx.Channel.GetMessagesAsync();

            foreach (var message in messages)
            {
                if (message.Author.Id == ctx.Client.CurrentUser.Id)
                {
                    botMessages.Add(message);
                }
            }

            await ctx.Channel.DeleteMessagesAsync(botMessages);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Clear channel!"));
        }

        [SlashCommand("profile", "get your profile info")]
        public async Task ProfileCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            string username = ctx.User.Username;

            var profileEmbed = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Azure)
                .WithTitle(username + "`s Profile")
                .WithThumbnail(ctx.User.AvatarUrl)
                );

            await ctx.Channel.SendMessageAsync(profileEmbed);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Here`s your profile"));
        }

    }
}
