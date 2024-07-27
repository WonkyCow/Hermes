using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using Microsoft.Data.Sqlite;
using DSharpPlus.Interactivity;
using Hermes.Commands.SlashCommands;

namespace Hermes
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        public static Database _database;

        static async Task Main(string[] args)
        {
            // Initialize the database
            _database = new Database("Data Source=messages.db");

            // Declare a new json reader
            var jsonReader = new JSONReader();

            // Call json reader
            await jsonReader.ReadJSON();

            // Set the Discord App Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            Client.Ready += Client_Ready;

            // Set the command configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDefaultHelp = true
            };

            // Initialize the CommandsNextExtension property
            Commands = Client.UseCommandsNext(commandsConfig);

            // Enable slash commands
            var slashCommandsConfiguration = Client.UseSlashCommands();

            // Register command class
            //Commands.RegisterCommands<TestCommand>();

            // Register slash commands
            slashCommandsConfiguration.RegisterCommands<Hermes.Commands.SlashCommands.pingCommand>();
            slashCommandsConfiguration.RegisterCommands<Hermes.Commands.SlashCommands.CreateCommands>(); 

            // Initialize the Interactivity extension
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5)
            });

            // Connect to the discord gateway
            await Client.ConnectAsync();

            // Log messages
            Client.MessageCreated += Client_MessageCreated;

            // Ensure the bot runs indefinitely, while the program is running
            await Task.Delay(-1);
        }

        // Logs messages to the database
        private static Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            _database.LogMessage(e.Author.Id, e.Guild.Id, e.Channel.Id, e.Message.Id, DateTime.UtcNow);
            return Task.CompletedTask;
        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            Console.WriteLine("Client is ready to process events.");
            return Task.CompletedTask;
        }
    }
}
