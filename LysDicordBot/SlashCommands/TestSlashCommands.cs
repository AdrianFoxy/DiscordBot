using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace LysDicordBot.SlashCommands
{
    public class TestSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("test", "first slash command")]
        public async Task TetsSlahCommand(InteractionContext ctx,
            [Choice("Pre-Defined Text", "123123asdasd")]
            [Option("string", "Type in anything you want")] string text)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent("Starting slash command..."));

            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = text
            };

            await ctx.Channel.SendMessageAsync(embed: embedMessage);
        }

        [SlashCommand("poll", "Create your own poll")]
        public async Task PollCommand(InteractionContext ctx,
            [Option("question", "The main poll subject/question")] string Question,
            [Option("timeLimit", "Time limit on this poll")] long TimeLimit,
            [Option("options", "options for pull")] string options,
            [Option("ImageUrl", "EnterImageUrl")] string? imageUrl
            )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                     .WithContent("Starting slash command..."));

            string[] optionArray = options.Split('*').Take(10).ToArray();

            var interactvity = ctx.Client.GetInteractivity();
            TimeSpan timer = TimeSpan.FromSeconds(TimeLimit);

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
                    .WithTitle(string.Join(" ", Question))
                    .WithDescription(GetOptionsDescription(optionArray, optionEmojis)));

            var putReachOn = await ctx.Channel.SendMessageAsync(pollMessage);

            for (int i = 0; i < optionArray.Length; i++)
            {
                DiscordEmoji emoji = optionEmojis[i];
                await putReachOn.CreateReactionAsync(emoji);
            }

            /*          foreach (var emoji in optionEmojis)
                        {
                            await putReachOn.CreateReactionAsync(emoji);
                        }

                        var result = await interactvity.CollectReactionsAsync(putReachOn, timer);

                        int count1 = 0;
                        int count2 = 0;
                        int count3 = 0;
                        int count4 = 0;


                        foreach (var emoji in result)
                        {
                            if (emoji.Emoji == optionEmojis[0]) count1++;
                            if (emoji.Emoji == optionEmojis[1]) count2++;
                            if (emoji.Emoji == optionEmojis[2]) count3++;
                            if (emoji.Emoji == optionEmojis[3]) count4++;
                        }

                        int totalVotes = count1 + count2 + count3 + count4;

                        var resultMessage = new DiscordMessageBuilder()
                            .AddEmbed(new DiscordEmbedBuilder()

                            .WithColor(DiscordColor.Green)
                            .WithTitle("Result of Pool")
                            .WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/9/91/Winkel_triple_projection_SW.jpg/1200px-Winkel_triple_projection_SW.jpg")
                            .WithDescription(($"Options: \n {optionEmojis[0]} | Votes {count1} |  \n" +
                                            $"{optionEmojis[1]} | Votes {count2} |  \n" +
                                            $"{optionEmojis[2]} | Votes {count3} |  \n" +
                                            $"{optionEmojis[3]} | Votes {count4} |  \n"))
                            );

                        await ctx.Channel.SendMessageAsync(resultMessage);*/
        }

        private string GetOptionsDescription(string[] options, DiscordEmoji[] emojis)
        {
            StringBuilder description = new StringBuilder("Options:\n");

            for (int i = 0; i < options.Length && i < emojis.Length; i++)
            {
                description.AppendLine($"{emojis[i]} - {options[i]}");
            }

            return description.ToString();
        }
    }
}
