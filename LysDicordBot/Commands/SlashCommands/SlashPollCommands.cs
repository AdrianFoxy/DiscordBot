using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using System.Diagnostics.Metrics;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.EventHandling;
using System.Collections.ObjectModel;

namespace LysDicordBot.Commands.SlashCommands
{
    public class SlashPollCommands : ApplicationCommandModule
    {
        [SlashCommand("poll", "create poll, enter question of pull, timeLimit, options and imageUrl if u need it")]
        public async Task CollectionCommand(InteractionContext ctx,
                        [Option("question", "The main poll subject/question")] string Question,
                        [Option("timeLimit", "Time limit on this poll")] long TimeLimit,
                        [Option("options", "Use * to enter multiple options")] string options,
                        [Option("ImageUrl", "Enter ImageUrl for pull, optional")] string imageUrl = null)
        {

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Starting poll..."));

            string[] optionArray = options.Split('*').Take(10).ToArray();

            DiscordEmoji[] optionEmojis =
            {
                DiscordEmoji.FromName(ctx.Client, ":one:", false),
                DiscordEmoji.FromName(ctx.Client, ":two:", false),
                DiscordEmoji.FromName(ctx.Client, ":three:", false),
                DiscordEmoji.FromName(ctx.Client, ":four:", false),
                DiscordEmoji.FromName(ctx.Client, ":five:", false),
                DiscordEmoji.FromName(ctx.Client, ":six:", false),
                DiscordEmoji.FromName(ctx.Client, ":seven:", false),
                DiscordEmoji.FromName(ctx.Client, ":eight:", false),
                DiscordEmoji.FromName(ctx.Client, ":nine:", false),
                DiscordEmoji.FromName(ctx.Client, ":keycap_ten:", false),
            };

            var pollMessage = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()
                            .WithColor(DiscordColor.Azure)
                            .WithImageUrl(imageUrl)
                            .WithTitle(string.Join(" ", Question))
                            .WithDescription(GetOptionsDescription(optionArray, optionEmojis)));

            var message = await ctx.Channel.SendMessageAsync(pollMessage);

            for (int i = 0; i < optionArray.Length; i++)
            {
                DiscordEmoji emoji = optionEmojis[i];
                await message.CreateReactionAsync(emoji);
            }
            var reactions = await message.CollectReactionsAsync(TimeSpan.FromSeconds(10));
            var resultMessage = new DiscordMessageBuilder();

            if (reactions.Count > 0)
            {
                var strBuilder = new StringBuilder("Options:\n\n");
                foreach (var reaction in reactions)
                {
                    var emojiIndex = Array.IndexOf(optionEmojis, reaction.Emoji);

                    strBuilder.AppendLine($"{reaction.Emoji} — {optionArray[emojiIndex]} | Total votes for option: {reaction.Total}");
                }

                strBuilder.AppendLine($"Total reactions :checkered_flag: : {reactions.Count}");
                resultMessage
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Green)
                    .WithImageUrl(imageUrl)
                    .WithTitle(string.Join(" ", Question))
                    .WithDescription(strBuilder.ToString())
                    );
            }
            else
            {
                resultMessage
                    .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Red)
                    .WithImageUrl("https://clipart-library.com/images/8TGbAKqqc.jpg")
                    .WithTitle("No votes, no poll :с")
                    );
            }
            await ctx.Channel.SendMessageAsync(resultMessage);
        }

        [SlashCommand("collect", "collect reaction during n-time")]
        public async Task CollectionCommand(InteractionContext ctx,
                                            [Option("timeLimit", "Time limit for colect")] long TimeLimit)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent("Starting collect..."));

            var message = await ctx.Channel.SendMessageAsync("React here!");

            var reactions = await message.CollectReactionsAsync(TimeSpan.FromSeconds(TimeLimit));
            int total = 0;
            var strBuilder = new StringBuilder();
            foreach (var reaction in reactions)
            {
                strBuilder.AppendLine($"{reaction.Emoji}: {reaction.Total}");
                total += reaction.Total;
            }

            strBuilder.AppendLine($"Total reactions: {total}");

            await ctx.Channel.SendMessageAsync(strBuilder.ToString());
        }

        [SlashCommand("flip", "flip coin, Eagle or Tails")]
        public async Task Flip(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent("Let`s flip..."));
            Random rand = new Random();
            int result = rand.Next(0, 2);
            string winner;
            if (result == 0) winner = "Eagle";
            else winner = "Tails";

            var winnerMessage = new DiscordEmbedBuilder()
            {
                Title = winner,
                Color = DiscordColor.Azure
            };
            await ctx.Channel.SendMessageAsync(embed: winnerMessage);
        }

        private string GetOptionsDescription(string[] options, DiscordEmoji[] emojis)
        {
            StringBuilder description = new StringBuilder("Options:\n\n");

            for (int i = 0; i < options.Length && i < emojis.Length; i++)
            {
                description.AppendLine($"{emojis[i]} — {options[i]}");
            }

            return description.ToString();
        }

    }
}
