using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace LysDicordBot.Commands
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

        [Command("flip")]
        public async Task Flip(CommandContext ctx)
        {
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

        [Command("poll")]
        public async Task PollCommand(CommandContext ctx, int TimeLimit, string Option1, string Option2, string Option3, string Option4, params string[] question)
        {
            var interactvity = ctx.Client.GetInteractivity();
            TimeSpan timer = TimeSpan.FromSeconds(TimeLimit);

            DiscordEmoji[] optionEmojis = { DiscordEmoji.FromName(ctx.Client, ":one:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":two:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":three:", false),
                                            DiscordEmoji.FromName(ctx.Client, ":four:", false), };

            string optionsString = optionEmojis[0] + " " + Option1 + " " + Option2 + " " + Option3 + " " + Option4;

            var pollMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle(string.Join(" ", question))
                .WithDescription($"Options: \n {optionEmojis[0]} | {Option1} \n" +
                                $"{optionEmojis[1]} | {Option2} \n" +
                                $"{optionEmojis[2]} | {Option3} \n" +
                                $"{optionEmojis[3]} | {Option4} \n")
                );

            var putReachOn = await ctx.Channel.SendMessageAsync(pollMessage);

            foreach (var emoji in optionEmojis)
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
                .WithDescription(($"Options: \n {optionEmojis[0]} | Votes {count1} | {Option1} \n" +
                                $"{optionEmojis[1]} | Votes {count2} | {Option2} \n" +
                                $"{optionEmojis[2]} | Votes {count3} | {Option3} \n" +
                                $"{optionEmojis[3]} | Votes {count4} | {Option4} \n"))
                );

            await ctx.Channel.SendMessageAsync(resultMessage);
        }
    }
}
