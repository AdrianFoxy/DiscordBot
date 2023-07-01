using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LysDicordBot.Commands.PrefixCommands;
using LysDicordBot.Commands.SlashCommands;
using DSharpPlus.Entities;

namespace LysDicordBot
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += OnClientReady;
            Client.ComponentInteractionCreated += ButtonPressResponse;
            Client.GuildMemberAdded += UserJoinHandler;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);


            var slashCommandsConfig = Client.UseSlashCommands();

            // Slash commands can be registered either globally or for a certain guild.
            // However, if you try to register them globally, they can take up to an hour to cache across all guilds.
            // So, it is recommended that you only register them for a certain guild for testing, and only register them globally once they're ready to be used.
            slashCommandsConfig.RegisterCommands<SlashPollCommands>(1071320251834306673);
            slashCommandsConfig.RegisterCommands<DataTableCommands>(1071320251834306673);
            slashCommandsConfig.RegisterCommands<HelpCommands>(1071320251834306673);


            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<CardGameCommands>();
            Commands.RegisterCommands<AdminCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private async Task UserJoinHandler(DiscordClient sender, GuildMemberAddEventArgs args)
        {
            var defaultChannel = args.Guild.GetDefaultChannel();

            var welcomeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Azure,
                Title = $"Welcome {args.Member.Username} to the podval",
                Description = "Come in don't be afraid, come out don't cry"
            };

            await defaultChannel.SendMessageAsync(embed: welcomeEmbed);
        }

        private async Task ButtonPressResponse(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            if(args.Interaction.Data.CustomId == "1")
            {
                await args.Interaction.CreateResponseAsync(
                    InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("```Poll command creates a discussion on the specified topic. \n\n" +
                                                                                                                                    "/poll <question/discission topic> <time limit in seconds, when the discussion will be ended> " +
                                                                                                                                    "<options for voting>, enter differents options using * " +
                                                                                                                                    "<ImageURL> if u wanna add some image to poll - just enter image url, this is optional```"));
            } else if(args.Interaction.Data.CustomId == "2")
            {
                await args.Interaction.CreateResponseAsync(
                   InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent("```Table commands help  you to create and manage table. \n\n" +
                                                                                                                                  "/create_table <table name> <table colums> to enter many columns  use | \n" +
                                                                                                                                  "/add_row <row value> use | to insert data to each column, also u can't enter more data than columns exists \n" +
                                                                                                                                  "/remove_row <row_number> IMPORTANT: line numbering starts from the first row you entered. Table name and columns can't be deleted \n" +
                                                                                                                                  "/delete_table will delete your table :( Think carefully before using this command. You can't get back deleted ones.\n" +
                                                                                                                                  "/get_table_txt you will get txt file with the current table \n" +
                                                                                                                                  "/get_logs_file you will get txt file with logs for the current table\n" +
                                                                                                                                  "/clear_table_commands Delete all table commands from the channel, except table```"));

            } else if(args.Interaction.Data.CustomId == "3")
            {

                await args.Interaction.CreateResponseAsync(
                   InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                   .AddEmbed(new DiscordEmbedBuilder()

                   .WithColor(DiscordColor.Yellow)
                   .WithTitle("Made by ©Lys")
                   .WithDescription("Use and enjoy :heart:")
                   .WithImageUrl("https://media.discordapp.net/attachments/1124661997485686834/1124667930454523994/Super_No_Ura_De_Yani_Suu_Hanashi5363.jpg")
                   ));
            }

        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
