using Discord;
using Discord.Commands;
using Hifumi.Addons;
using System;
using System.Threading.Tasks;

#warning Temporary commands. Do not include in build

namespace Hifumi.Modules
{
    [Name("Temporary Non-shippable Commands"), RequireOwner]
    public class NoBuildModule : Base
    {
        [Command("shutdown")]
        public Task Shutdown()
        {
            Environment.Exit(Environment.ExitCode);
            return Task.CompletedTask;
        }
    }
}
