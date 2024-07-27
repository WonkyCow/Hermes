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

namespace Hermes
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Messages (
                    UserId TEXT,
                    GuildId TEXT,
                    ChannelId TEXT,
                    MessageId TEXT,
                    Timestamp TEXT
                )";
                command.ExecuteNonQuery();
            }
        }

        public void LogMessage(ulong userId, ulong guildId, ulong channelId, ulong messageId, DateTime timestamp)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO Messages (UserId, GuildId, ChannelId, MessageId, Timestamp)
                VALUES ($userId, $guildId, $channelId, $messageId, $timestamp)";
                command.Parameters.AddWithValue("$userId", userId.ToString());
                command.Parameters.AddWithValue("$guildId", guildId.ToString());
                command.Parameters.AddWithValue("$channelId", channelId.ToString());
                command.Parameters.AddWithValue("$messageId", messageId.ToString());
                command.Parameters.AddWithValue("$timestamp", timestamp.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        public int GetTotalMessages(ulong userId, ulong guildId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT COUNT(*) FROM Messages
                WHERE UserId = $userId";
                command.Parameters.AddWithValue("$userId", userId.ToString());
                command.Parameters.AddWithValue("$guildId", guildId.ToString());

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int GetMessagesInLast30Days(ulong userId, ulong guildId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT COUNT(*) FROM Messages
                WHERE UserId = $userId AND Timestamp >= $thirtyDaysAgo";
                command.Parameters.AddWithValue("$userId", userId.ToString());
                command.Parameters.AddWithValue("$guildId", guildId.ToString());
                command.Parameters.AddWithValue("$thirtyDaysAgo", DateTime.UtcNow.AddDays(-30).ToString("o"));

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public DateTime? GetLastMessageTimestamp(ulong userId, ulong guildId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT MAX(Timestamp) FROM Messages
                WHERE UserId = $userId";
                command.Parameters.AddWithValue("$userId", userId.ToString());
                command.Parameters.AddWithValue("$guildId", guildId.ToString());

                var result = command.ExecuteScalar()?.ToString();
                if (DateTime.TryParse(result, out DateTime timestamp))
                {
                    return timestamp;
                }
                return null;
            }
        }
        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                SELECT DISTINCT UserId, DisplayName, Username
                FROM Messages";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            UserId = ulong.Parse(reader.GetString(0)),
                            DisplayName = reader.GetString(1),
                            Username = reader.GetString(2)
                        };
                        users.Add(user);
                    }
                }
            }
            return users;
        }
        public class User
        {
            public ulong UserId { get; set; }
            public string DisplayName { get; set; }
            public string Username { get; set; }
        }
    }

}
