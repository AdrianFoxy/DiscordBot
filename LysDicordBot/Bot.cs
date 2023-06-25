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
using LysDicordBot.SlashCommands;
using LysDicordBot.Commands;

namespace LysDicordBot
{
    public class Bot : IDisposable
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
            slashCommandsConfig.RegisterCommands<TestSlashCommands>(1071320251834306673);

            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<CardGameCommands>();
            Commands.RegisterCommands<HelpCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
