using Discord.Commands;
using Hifumi.Helpers;
using System;
using System.Threading.Tasks;

using Hifumi.Enums;
using Hifumi.Services;
using System.Drawing;

namespace Hifumi.Addons.Preconditions
{
    public class RequireCooldown : PreconditionAttribute
    {
        TimeSpan cool;

        public RequireCooldown(int seconds) => cool = TimeSpan.FromSeconds(seconds);

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext commandContext, CommandInfo command, IServiceProvider serviceProvider)
        {
            var context = commandContext as IContext;
            var profile = context.GuildHelper.GetProfile(context.Guild.Id, context.User.Id);
            if (!profile.Commands.ContainsKey(command.Name)) return Task.FromResult(PreconditionResult.FromSuccess());

            var passed = DateTime.UtcNow - profile.Commands[command.Name];
            if (passed >= cool) return Task.FromResult(PreconditionResult.FromSuccess());

            var wait = cool - passed;
            // TODO: personality and don't send minutes if there are none
            return Task.FromResult(PreconditionResult.FromError($"Please wait {wait.Minutes} minute(s) and {wait.Seconds} second(s)."));
        }
    }
}
