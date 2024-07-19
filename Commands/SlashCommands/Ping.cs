using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Threading.Tasks;

namespace Hermes.Commands.SlashCommands
{
    public class pingCommand : ApplicationCommandModule
    {
        [SlashCommand("ping", "Pings the bot for a response")]
        public async Task PingSlashCommand(InteractionContext context)
        {
            try
            {
                // Defer the interaction to acknowledge receipt of the command
                await context.DeferAsync();
                Console.WriteLine("DeferAsync successful");

                // Send the result to the channel where the command was invoked
                await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Pong!"));
                Console.WriteLine("EditResponseAsync successful");
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.ToString());
                await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while processing the command."));
            }
        }
    }
}