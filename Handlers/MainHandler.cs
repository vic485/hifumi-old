using Discord;
using Discord.WebSocket;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi.Handlers
{
    public class MainHandler
    {
        ConfigHandler ConfigHandler { get; }
        HttpClient HttpClient { get; }
        DiscordSocketClient Client { get; }

        public MainHandler(ConfigHandler configHandler, HttpClient httpClient, DiscordSocketClient client)
        {
            ConfigHandler = configHandler;
            HttpClient = httpClient;
            Client = client;
        }

        public async Task InitializeAsync()
        {
            await DatabaseCheck();

            // TODO: Event linking

            await Client.LoginAsync(TokenType.Bot, ConfigHandler.Config.Token).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
        }

        async Task DatabaseCheck()
        {
            try
            {
                var raven = await HttpClient.GetAsync("http://localhost:8080/studio/index.html").ConfigureAwait(false);
                var database = await HttpClient.GetAsync("http://localhost:8080/studio/index.html#databases/documents?&database=Hifumi").ConfigureAwait(false);
                if (raven.IsSuccessStatusCode || database.IsSuccessStatusCode) ConfigHandler.ConfigCheck();
            }
            catch
            {
                Console.WriteLine("Could not connect to database."); // TODO: Log?
                await Task.Delay(5000);
                Environment.Exit(Environment.ExitCode);
            }
        }
    }
}
