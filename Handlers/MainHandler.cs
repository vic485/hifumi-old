using Discord;
using Discord.WebSocket;
using Hifumi.Enums;
using Hifumi.Services;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CC = System.Drawing.Color;

namespace Hifumi.Handlers
{
    public class MainHandler
    {
        ConfigHandler ConfigHandler { get; }
        HttpClient HttpClient { get; }
        IDocumentStore Store { get; }
        DiscordSocketClient Client { get; }
        EventsHandler EventsHandler { get; }

        public MainHandler(ConfigHandler configHandler, HttpClient httpClient, EventsHandler eventsHandler, DiscordSocketClient client, IDocumentStore store)
        {
            ConfigHandler = configHandler;
            HttpClient = httpClient;
            Store = store;
            Client = client;
            EventsHandler = eventsHandler;
        }

        public async Task InitializeAsync()
        {
            await DatabaseCheck();

            Client.Log += EventsHandler.Log;
            Client.Ready += EventsHandler.Ready;
            Client.LeftGuild += EventsHandler.LeftGuild;
            Client.Connected += EventsHandler.Connected;
            Client.UserLeft += EventsHandler.UserLeftAsync;
            Client.Disconnected += EventsHandler.Disconnected;
            Client.GuildAvailable += EventsHandler.GuildAvailable;
            Client.UserJoined += EventsHandler.UserJoinedAsync;
            Client.JoinedGuild += EventsHandler.JoinedGuildAsync;
            Client.LatencyUpdated += EventsHandler.LatencyUpdated;
            Client.ReactionAdded += EventsHandler.ReactionAddedAsync;
            Client.MessageReceived += EventsHandler.HandleMessage;
            Client.MessageDeleted += EventsHandler.MessageDeletedAsync;
            Client.ReactionRemoved += EventsHandler.ReactionRemovedAsync;
            Client.MessageReceived += EventsHandler.CommandHandlerAsync;

            AppDomain.CurrentDomain.UnhandledException += EventsHandler.UnhandledException;

            await Client.LoginAsync(TokenType.Bot, ConfigHandler.Config.Token).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
        }

        async Task DatabaseCheck()
        {
            var database = await DatabaseHandler.LoadDBConfigAsync();
            try
            {
                if (!Store.Maintenance.Server.Send(new GetDatabaseNamesOperation(0, 5)).Any(x => x == database.DatabaseName))
                {
                    LogService.Write(LogSource.DTB, $"No database named {database.DatabaseName} found! Creating database {database.DatabaseName}...", CC.IndianRed);
                    await Store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database.DatabaseName)));
                    LogService.Write(LogSource.DTB, $"Created Database {database.DatabaseName}.", CC.ForestGreen);
                }
            }
            catch { }
            finally
            {
                ConfigHandler.ConfigCheck();
            }
        }
    }
}
