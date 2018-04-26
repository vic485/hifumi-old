using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hifumi
{
    class Hifumi
    {
        static void Main(string[] args) => new Hifumi().InitializeAsync().GetAwaiter().GetResult();

        async Task InitializeAsync()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 20,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Error
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    ThrowOnError = true,
                    IgnoreExtraArgs = false,
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton(new DocumentStore
                {
                    Database = "Hifumi",
                    Urls = new[] { "http://localhost:8080" }
                }.Initialize())
                .AddSingleton<HttpClient>()
                .AddSingleton<MainHandler>()
                .AddSingleton<ConfigHandler>()
                .AddSingleton(new Random(Guid.NewGuid().GetHashCode()));

            var provider = services.BuildServiceProvider();
            await provider.GetRequiredService<MainHandler>().InitializeAsync();

            await Task.Delay(-1);
        }
    }
}
