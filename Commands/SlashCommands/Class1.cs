using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace hermes
{
    internal class AuditCreator
    {

    }
    internal class pingCommand
    {
        [SlashCommand("ping", "Pings the bot for a response")]
        public async Task pingSlashCommand(InteractionContext context)
        {
            try
            {
                // Defer the interaction to acknowledge receipt of the command
                await context.DeferAsync();
                Console.WriteLine("DerferAsync successful");

                // Perform the ping check
                string result = await CheckForArbitrage();
                Console.WriteLine("CheckForArbitrage successful");

                // Send the result to the channel where the command was invoked
                await SendMessageToChannel(context, result);
                Console.Write($"result received: {result}\n");
                Console.WriteLine("SendMessageToChannel successful");

            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine(ex.ToString());
                await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An error occurred while checking for arbitrage."));
            }
        }
    }
}
}   