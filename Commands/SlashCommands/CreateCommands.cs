using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hermes.Commands.SlashCommands
{
    internal class CreateCommands : ApplicationCommandModule
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

                    var auditFields = new List<(string name, string value)>();
                    foreach (var member in membersWithRole)
                    {
                        var displayName = member.DisplayName;
                        var username = member.Username;
                        var accountId = member.Id;

                        var totalMessages = Program._database.GetTotalMessages(accountId, context.Guild.Id);
                        var messagesInLast30Days = Program._database.GetMessagesInLast30Days(accountId, context.Guild.Id);
                        var lastMessageSent = Program._database.GetLastMessageTimestamp(accountId, context.Guild.Id);

                        var lastMessageSentFormatted = lastMessageSent.HasValue
                            ? $"<t:{((DateTimeOffset)lastMessageSent.Value.ToUniversalTime()).ToUnixTimeSeconds()}:f>"
                            : "No messages";

                        auditFields.Add(($"Member: {displayName} ({username})",
                            $"Account ID: {accountId}\n" +
                            $"Total Messages: {totalMessages}\n" +
                            $"Messages in Last 30 Days: {messagesInLast30Days}\n" +
                            $"Last Message Sent: {lastMessageSentFormatted}"));
                    }

                    if (auditFields.Count == 0)
                    {
                        await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No data found for this role."));
                        return;
                    }

                    var pages = CreateEmbedPages(auditFields, $"Audit for Role: {role.Name}", role.Color, membersWithRole.Count);
                    var currentPage = 0;

                    var builder = new DiscordWebhookBuilder()
                        .AddEmbed(pages[currentPage])
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "previous", "Previous", disabled: currentPage == 0),
                                       new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next", disabled: currentPage == pages.Count - 1));

                    var response = await context.EditResponseAsync(builder);

                    var interactivity = context.Client.GetInteractivity();

                    if (interactivity == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Interactivity is not initialized.");
                        Console.ResetColor();
                        return;
                    }

                    while (true)
                    {
                        var buttonResponse = await interactivity.WaitForButtonAsync(response, TimeSpan.FromMinutes(5));

                        if (buttonResponse.TimedOut)
                            break;

                        if (buttonResponse.Result.Id == "previous")
                        {
                            currentPage = Math.Max(currentPage - 1, 0);
                        }
                        else if (buttonResponse.Result.Id == "next")
                        {
                            currentPage = Math.Min(currentPage + 1, pages.Count - 1);
                        }

                        var updatedBuilder = new DiscordWebhookBuilder()
                            .AddEmbed(pages[currentPage])
                            .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "previous", "Previous", disabled: currentPage == 0),
                                           new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next", disabled: currentPage == pages.Count - 1));

                        await buttonResponse.Result.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(updatedBuilder));
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while creating the audit."));
                }
            }


            [SlashCommand("badnamelist", "Creates a list of users with bad/unpingable display names")]
            public async Task CreateBadNameListSlashCommand(InteractionContext context)
            {
                try
                {
                    await context.DeferAsync();

                    var hardCodedLetters = @"!@#$%^&*()_+1234567890-=QWERTYUIOP{}|ASDFGHJKL:""ZXCVBNM<>?qwertyuiop[]\asdfghjkl;''zxcvbnm,./~`'";

                    var allMembers = context.Guild.Members.Values.ToList();
                    var badNameFields = new List<(string name, string value)>();

                    foreach (var member in allMembers)
                    {
                        var displayName = member.DisplayName;
                        if (!PingableCheck(displayName, hardCodedLetters))
                        {
                            badNameFields.Add(($"Member: {displayName}",
                                $"Account ID: {member.Id}\n" +
                                $"Username: {member.Username}"));
                        }
                    }

                    if (badNameFields.Count == 0)
                    {
                        await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("No bad names found."));
                        return;
                    }

                    var pages = CreateEmbedPages(badNameFields, "Bad Name List", DiscordColor.Red, badNameFields.Count);
                    var currentPage = 0;

                    var builder = new DiscordWebhookBuilder()
                        .AddEmbed(pages[currentPage])
                        .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "previous", "Previous", disabled: currentPage == 0),
                                       new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next", disabled: currentPage == pages.Count - 1));

                    var response = await context.EditResponseAsync(builder);

                    var interactivity = context.Client.GetInteractivity();

                    if (interactivity == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Interactivity is not initialized.");
                        Console.ResetColor();
                        return;
                    }

                    while (true)
                    {
                        var buttonResponse = await interactivity.WaitForButtonAsync(response, TimeSpan.FromMinutes(5));

                        if (buttonResponse.TimedOut)
                            break;

                        if (buttonResponse.Result.Id == "previous")
                        {
                            currentPage = Math.Max(currentPage - 1, 0);
                        }
                        else if (buttonResponse.Result.Id == "next")
                        {
                            currentPage = Math.Min(currentPage + 1, pages.Count - 1);
                        }

                        var updatedBuilder = new DiscordWebhookBuilder()
                            .AddEmbed(pages[currentPage])
                            .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "previous", "Previous", disabled: currentPage == 0),
                                           new DiscordButtonComponent(ButtonStyle.Primary, "next", "Next", disabled: currentPage == pages.Count - 1));

                        await buttonResponse.Result.Interaction.CreateResponseAsync(DSharpPlus.InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(updatedBuilder));
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                    await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while creating the bad name list."));
                }
            }


            private bool PingableCheck(string name, string letters)
            {
                for (int i = 0; i < name.Length - 2; i++)
                {
                    if (letters.Contains(name[i]) && letters.Contains(name[i + 1]) && letters.Contains(name[i + 2]))
                    {
                        return true;
                    }
                }
                return false;
            }

            private List<DiscordEmbedBuilder> CreateEmbedPages(List<(string name, string value)> fields, string title, DiscordColor color, int totalUsers)
            {
                var pages = new List<DiscordEmbedBuilder>();
                var currentEmbed = new DiscordEmbedBuilder { Title = title, Color = color };
                int fieldCount = 0;

                foreach (var field in fields)
                {
                    if (fieldCount >= 10)
                    {
                        fieldCount = 0;
                        pages.Add(currentEmbed);
                        currentEmbed = new DiscordEmbedBuilder { Title = title, Color = color };
                    }
                    currentEmbed.AddField(field.name, field.value, false);
                    fieldCount++;
                }

                if (currentEmbed.Fields.Count > 0)
                {
                    pages.Add(currentEmbed);
                }

                // Add page numbers and total users to footer
                for (int i = 0; i < pages.Count; i++)
                {
                    pages[i].WithFooter($"Page {i + 1}/{pages.Count} | Total Users: {totalUsers}");
                }

                return pages;
            }

        }
    }
}
