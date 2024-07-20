using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hermes.Commands.SlashCommands
{
    internal class CreateAuditCommand : ApplicationCommandModule
    {
        [SlashCommandGroup("create", "Group for create commands")]
        public class CreateCommandGroup : ApplicationCommandModule
        {
            [SlashCommand("audit", "Creates an audit for a specified role")]
            public async Task CreateAuditSlashCommand(InteractionContext context,
                [Option("role", "The role to audit")] DiscordRole role)
            {
                try
                {
                    await context.DeferAsync();

                    var membersWithRole = context.Guild.Members.Values
                        .Where(member => member.Roles.Any(r => r.Id == role.Id))
                        .ToList();

                    var auditData = new DiscordEmbedBuilder
                    {
                        Title = $"Audit for Role: {role.Name}",
                        Color = DiscordColor.Blurple
                    };

                    foreach (var member in membersWithRole)
                    {
                        var displayName = member.DisplayName;
                        var username = member.Username;
                        var accountId = member.Id;

                        var totalMessages = Program._database.GetTotalMessages(accountId);
                        var messagesInLast30Days = Program._database.GetMessagesInLast30Days(accountId);
                        var lastMessageSent = Program._database.GetLastMessageTimestamp(accountId);

                        auditData.AddField($"Member: {displayName} ({username})",
                            $"Account ID: {accountId}\n" +
                            $"Total Messages: {totalMessages}\n" +
                            $"Messages in Last 30 Days: {messagesInLast30Days}\n" +
                            $"Last Message Sent: {lastMessageSent?.ToString("g") ?? "No messages"}", false);
                    }

                    await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(auditData));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Audit Command Ran Successfully");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while creating the audit."));
                }
            }     
        }
    }
}
