using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace LysDicordBot.Commands.SlashCommands
{
    public class DataTableCommands : ApplicationCommandModule
    {
        private static ulong LastMessageId;
        private static string _tableFilePath = Path.Combine(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "table"), "table.txt"));
        private static string _idFilePath = Path.Combine(Path.Combine(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "table"), "TableMessageId.txt"));
        private static string _folderPath = Path.Combine(Path.GetDirectoryName(Directory.GetCurrentDirectory()), "table");

        [SlashCommand("get_table_txt", "download txt file of table")]
        public async Task SendTableFileCommand(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                     .WithContent("Here is your file..."));
            if (File.Exists(_tableFilePath))
            {
                using(var fs = new FileStream(_tableFilePath, FileMode.Open, FileAccess.Read)){

                    var messagebuilder = new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder()
                        .WithTitle("Here is your table o/"))
                        .AddFile(fs);
                    await ctx.Channel.SendMessageAsync(messagebuilder);
                }
            }
        }


        [SlashCommand("add_row", "add_row to table, insert new value of the row")]
        public async Task AddRow(InteractionContext ctx,
                                 [Option("ValueOfRows", "Enter the value of the row, which correspond to the table. To start new row, use |")] string ValuesOfRow
                                 )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent("Adding new row..."));

            if (!File.Exists(_tableFilePath)) { await SendErrorMessage(ctx, "Error: Table doesn`t exists."); return; }

            string[] fileLines = File.ReadAllLines(_tableFilePath);
            string TableFields;

            if (fileLines.Length >= 2) TableFields = fileLines[1];
            else { await SendErrorMessage(ctx, "Can`t find table fields."); return; }

            string[] tableFields = TableFields.Split('|').Take(10).ToArray();
            string[] valuesOfRow = ValuesOfRow.Split('|').Take(tableFields.Length-1).ToArray();

            string newRow = string.Join("|", valuesOfRow);
            newRow += "|" + DateTime.Now.ToString();
            File.AppendAllText(_tableFilePath, newRow + Environment.NewLine);
            string updatedContent = File.ReadAllText(_tableFilePath);

            if (LastMessageId == 0) LastMessageId = GetTableMsgId();
            var tableMessage = await ctx.Channel.GetMessageAsync(LastMessageId);

            if (tableMessage != null)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Description = updatedContent
                };

                await tableMessage.ModifyAsync(properties =>
                {
                    properties.Embed = embed.Build();
                });
            }

            await SendSuccessMessage(ctx, "New row added");
        }

        [SlashCommand("delete_table", "delete_table")]
        public async Task DeleteTable(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Deleting table..."));

            if (!File.Exists(_tableFilePath)) { await SendErrorMessage(ctx, "Table file doesn`t exists."); return; }
            File.Delete(_tableFilePath);

            if (LastMessageId == 0) LastMessageId = GetTableMsgId();
            if (!File.Exists(_idFilePath)) { await SendErrorMessage(ctx, "Can`t find and delete table message."); return; }
            File.Delete(_idFilePath);

            var tableMessage = await ctx.Channel.GetMessageAsync(LastMessageId);
            await tableMessage.DeleteAsync();
            LastMessageId = 0;
            await SendSuccessMessage(ctx, "Table deleted.");
        }

        [SlashCommand("start_table", "start_table")]
        public async Task StartTable(InteractionContext ctx,
                                    [Option("Name", "Enter table name.")] string TableName,
                                    [Option("Fields", "Use | to make several fields. Max fields num is 10.")] string TableFields
            )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                      .WithContent("Starting table..."));

            if (File.Exists(_tableFilePath))  { await SendErrorMessage(ctx, "Error: Table file already exists."); return; }

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
        private async Task SendErrorMessage(InteractionContext ctx, string errorMessage)
        {
            var errorEmbed = new DiscordEmbedBuilder()
            {
                Title = errorMessage,
                Color = DiscordColor.Red,
            };

            await ctx.Channel.SendMessageAsync(embed: errorEmbed);
        }

        private async Task SendSuccessMessage(InteractionContext ctx, string successMessage)
        {
            var errorEmbed = new DiscordEmbedBuilder()
            {
                Title = successMessage,
                Color = DiscordColor.Green,
            };

            await ctx.Channel.SendMessageAsync(embed: errorEmbed);
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
    }
}
