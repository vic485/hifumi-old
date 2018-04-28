using Discord;
using Discord.Commands;
using Hifumi.Addons;
using System;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Fun Commands"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class FunModule : Base
    {
        [Command("clap"), Summary("Replaces spaces in your message with the clap emoji.")]
        public Task Clap([Remainder] string message) => ReplyAsync($"👏 {message.Replace(" ", " 👏 ")} 👏");

        [Command("clapd"), Summary("Deletes your message and replaces spaces in your message with the clap emoji.")]
        public async Task ClapDeleteAsync([Remainder] string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"👏 {message.Replace(" ", " 👏 ")} 👏");
        }
    }
}
