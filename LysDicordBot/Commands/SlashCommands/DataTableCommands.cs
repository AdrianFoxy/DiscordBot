using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LysDicordBot.Interfaces;
using LysDicordBot.Models;
using LysDicordBot.Services;

namespace LysDicordBot.Commands.SlashCommands
{
    public class DataTableCommands : ApplicationCommandModule
    {
        private static ulong LastMessageId;

        private static string _tableFilePath = Path.Combine(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "table"), "table.txt"));
        private static string _idFilePath = Path.Combine(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "table"), "TableMessageId.txt"));
        private static string _folderPath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "table");

        private static string _logFilePath = Path.Combine(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "table"), "log.txt"));

        LogService _logservice = new LogService(_logFilePath);

        [SlashCommand("get_table_txt", "download txt file of table")]
        public async Task SendTableFileCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (File.Exists(_tableFilePath))
            {
                using (var fs = new FileStream(_tableFilePath, FileMode.Open, FileAccess.Read))
                {

                    var messagebuilder = new DiscordMessageBuilder()
                        .AddFile(fs);
                    await ctx.Channel.SendMessageAsync(messagebuilder);
                }
                _logservice.Write(new TableLog(DateTime.Now, ctx.CommandName, ctx.User.Username).ToString());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Here is your file:"));
            } else {  await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No table file")); }
        }

        [SlashCommand("get_logs_file", "download txt file of logs")]
        public async Task SendLogsFile(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (File.Exists(_logFilePath))
            {
                using (var fs = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read))
                {

                    var messagebuilder = new DiscordMessageBuilder()
                        .AddFile(fs);
                    await ctx.Channel.SendMessageAsync(messagebuilder);
                }
                _logservice.Write(new TableLog(DateTime.Now, ctx.CommandName, ctx.User.Username).ToString());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Here is your file:"));
            } else{ await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No log file")); }
        }

        [SlashCommand("clear_bot_commands", "delete all table commands, except table")]
        public async Task Clear(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            var botMessages = new List<DiscordMessage>();
            var messages = await ctx.Channel.GetMessagesAsync();
            if(LastMessageId == 0) LastMessageId = GetTableMsgId();

            foreach (var message in messages)
            {
                if (message.Author.Id == ctx.Client.CurrentUser.Id && message.Id != LastMessageId)
                {
                    botMessages.Add(message);
                }
            }

            await ctx.Channel.DeleteMessagesAsync(botMessages);
            _logservice.Write(new TableLog(DateTime.Now, ctx.CommandName, ctx.User.Username).ToString());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Clear channel!"));
        }

        [SlashCommand("remove_row", "remove row by #")]
        public async Task RemoveRow(InteractionContext ctx,
                                    [Option("NumOfRow", "Enter # of row, that u wanna remove")] long RowNumber)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (!File.Exists(_tableFilePath)) { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table doesn`t exists!")); }

            string[] lines = await File.ReadAllLinesAsync(_tableFilePath);
            RowNumber += 2;
            if (RowNumber >= 1 && RowNumber <= lines.Length)
            {
                List<string> modifiedLines = lines.ToList();
                modifiedLines.RemoveAt((int)RowNumber - 1);

                // Save data
                await File.WriteAllLinesAsync(_tableFilePath, modifiedLines);
                _logservice.Write(new TableLog(DateTime.Now, $"{ctx.CommandName} {RowNumber}", ctx.User.Username).ToString());
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Row removed!"));

                await SortTable();

                // Read new file data, without 1st row with name
                string[] newFileLines = File.ReadAllLines(_tableFilePath);
                string updatedContent = string.Join(Environment.NewLine, newFileLines.Skip(1));

                if (LastMessageId == 0) LastMessageId = GetTableMsgId();
                var tableMessage = await ctx.Channel.GetMessageAsync(LastMessageId);

                // Get old embed Title
                string currentTitle = string.Empty;
                if (tableMessage != null)
                {
                    currentTitle = tableMessage.Embeds.FirstOrDefault()?.Title ?? string.Empty;
                }

                if (tableMessage != null)
                {
                    var embed = new DiscordEmbedBuilder()
                    {
                        Title = currentTitle,
                        Description = updatedContent,
                        Color = DiscordColor.Azure
                    };

                    await tableMessage.ModifyAsync(properties =>
                    {
                        properties.Embed = embed.Build();
                    });
                }

            }
            else await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("There is no row with this number!"));


        }

        [SlashCommand("add_row", "add_row to table, insert new value of the row")]
        public async Task AddRow(InteractionContext ctx,
                                 [Option("ValueOfRows", "Enter the value of the row, which correspond to the table. To start new row, use |")] string ValuesOfRow
                                 )
        {

            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (!File.Exists(_tableFilePath)) { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table doesn`t exists!")); }

            string[] fileLines = File.ReadAllLines(_tableFilePath);
            string TableFields;


            // Check number of fields, coudnt enter more data then main fields
            if (fileLines.Length >= 2) TableFields = fileLines[1];
            else { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Can`t find table fields!")); return; }

            string[] tableFields = TableFields.Split('|').Take(10).ToArray();
            string[] valuesOfRow = ValuesOfRow.Split('|').Take(tableFields.Length-1).ToArray();

            // Update data in file + DateTime field
            string newRow = string.Join("|", valuesOfRow);
            newRow += "|" + DateTime.Now.ToString();
            File.AppendAllText(_tableFilePath, newRow + Environment.NewLine);

            await SortTable();

            // Read new file data, without 1st row with name
            string[] newFileLines = File.ReadAllLines(_tableFilePath);
            string updatedContent = string.Join(Environment.NewLine, newFileLines.Skip(1));

            // Get table message id for update
            if (LastMessageId == 0) LastMessageId = GetTableMsgId();
            var tableMessage = await ctx.Channel.GetMessageAsync(LastMessageId);

            // Get old embed Title
            string currentTitle = string.Empty;
            if (tableMessage != null)
            {
                currentTitle = tableMessage.Embeds.FirstOrDefault()?.Title ?? string.Empty;
            }

            // Update message with new data
            if (tableMessage != null)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = currentTitle,
                    Description = updatedContent,
                    Color = DiscordColor.Azure,
                };

                await tableMessage.ModifyAsync(properties =>
                {
                    properties.Embed = embed.Build();
                });
            }

            _logservice.Write(new TableLog(DateTime.Now, $"{ctx.CommandName} {ValuesOfRow}", ctx.User.Username).ToString());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("New row added!"));

        }

        [SlashCommand("delete_table", "delete_table")]
        public async Task DeleteTable(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (!File.Exists(_tableFilePath)) { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table doesn`t exists!")); }
            File.Delete(_tableFilePath);

            if (LastMessageId == 0) LastMessageId = GetTableMsgId();
            if (!File.Exists(_idFilePath)) { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table doesn`t exists!")); }
            File.Delete(_idFilePath);

            var tableMessage = await ctx.Channel.GetMessageAsync(LastMessageId);
            await tableMessage.DeleteAsync();
            LastMessageId = 0;
            _logservice.Write(new TableLog(DateTime.Now, ctx.CommandName, ctx.User.Username).ToString());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table deleted!"));
        }

        [SlashCommand("create_table", "create new table. Enter name and filds for enter.")]
        public async Task StartTable(InteractionContext ctx,
                                    [Option("Name", "Enter table name.")] string TableName,
                                    [Option("Fields", "Use | to make several fields. Max fields num is 10.")] string TableFields
            )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (File.Exists(_tableFilePath))  { await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table doesn`t exists!")); }

            Directory.CreateDirectory(_folderPath);
            string[] tableFields = TableFields.Split('|').Take(10).ToArray();
            string joinedTableFields = string.Join("|", tableFields);
            joinedTableFields += " | DateTime";

            using (FileStream fileStream = File.Create(_tableFilePath))
            {
                // Write TableName
                byte[] tableNameBytes = Encoding.UTF8.GetBytes(TableName + Environment.NewLine);
                fileStream.Write(tableNameBytes, 0, tableNameBytes.Length);

                // Write TableFields
                byte[] tableFieldsBytes = Encoding.UTF8.GetBytes(joinedTableFields + Environment.NewLine);
                fileStream.Write(tableFieldsBytes, 0, tableFieldsBytes.Length);
            }

            await ShowTable(ctx, TableName, joinedTableFields);

            using (FileStream fileStream = File.Create(_idFilePath))
            {
                byte[] tablemsgID = Encoding.UTF8.GetBytes(LastMessageId + Environment.NewLine);
                fileStream.Write(tablemsgID, 0, tablemsgID.Length);
            }

            _logservice.Write(new TableLog(DateTime.Now, ctx.CommandName, ctx.User.Username).ToString());
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Table created!"));
        }

        private ulong GetTableMsgId()
        {
            if (File.Exists(_idFilePath))
            {
                string fileContent = File.ReadAllText(_idFilePath);
                if (!string.IsNullOrEmpty(fileContent) && ulong.TryParse(fileContent, out ulong result)) return result;
            }
            return 0;
        }

        private async Task ShowTable(InteractionContext ctx, string TableName, string TableFields)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = TableName,
                Description = TableFields,
                Color = DiscordColor.Azure,
            };

            var sendedMessage = await ctx.Channel.SendMessageAsync(embed: embedMessage);
            if(LastMessageId == 0) LastMessageId = sendedMessage.Id;
        }

        private async Task SortTable()
        {
            string[] lines = File.ReadAllLines(_tableFilePath);
            List<string[]> table = new List<string[]>();
            for (int i = 2; i < lines.Length; i++)
            {
                string[] values = lines[i].Split('|');
                table.Add(values);
            }
            // Sort by alphabet
            table.Sort((x, y) => string.Compare(x[0], y[0], StringComparison.OrdinalIgnoreCase));
            // Data to write
            List<string> outputLines = new List<string>();
            outputLines.Add(lines[0]);
            outputLines.Add(lines[1]);
            // Write everything to file
            foreach (string[] row in table) outputLines.Add(string.Join("|", row));
            File.WriteAllLines(_tableFilePath, outputLines);
        }
    }
}
