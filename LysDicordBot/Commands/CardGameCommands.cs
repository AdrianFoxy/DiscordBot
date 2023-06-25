using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LysDicordBot.ExternalClasses;

namespace LysDicordBot.Commands
{
    public class CardGameCommands : BaseCommandModule
    {
        [Command("card_game")]
        public async Task SimpleCardGame(CommandContext ctx)
        {
            var UserCard = new CardBuilder();

            var userCardMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("Your card")
                .WithDescription($"You drew a: {UserCard.SelectedCard}")
                );

            await ctx.Channel.SendMessageAsync(userCardMessage);

            var BotCard = new CardBuilder();

            var botCardMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()

                .WithColor(DiscordColor.Azure)
                .WithTitle("Bot card")
                .WithDescription($"The bot drew a: {BotCard.SelectedCard}")
                );

            await ctx.Channel.SendMessageAsync(botCardMessage);

            string winner = "Who?";
            if (UserCard.SelectedNumber > BotCard.SelectedNumber) winner = "Congrats champ!";
            else if (UserCard.SelectedNumber < BotCard.SelectedNumber) winner = "Bot won. Someone doubted?";
            else winner = "Draw";


            var winnerMessage = new DiscordEmbedBuilder()
            {
                Title = winner,
                Color = DiscordColor.Green
            };
            await ctx.Channel.SendMessageAsync(embed: winnerMessage);
        }
    }
}
