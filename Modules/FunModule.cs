using Discord;
using Discord.Commands;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using Hifumi.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
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

        [Command("expand"), Summary("Converts text to full width.")]
        public Task Expand([Remainder] string message)
            => ReplyAsync(string.Join("", message.Select(x => StringHelper.Normal.Contains(x) ? x : ' ').Select(x => StringHelper.FullWidth[StringHelper.Normal.IndexOf(x)])));

        [Command("expandd"), Summary("Deletes your message and converts the text to full width.")]
        public async Task ExpandAsync([Remainder] string message)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(string.Join("", message.Select(x => StringHelper.Normal.Contains(x) ? x : ' ').Select(x => StringHelper.FullWidth[StringHelper.Normal.IndexOf(x)])));
        }

        [Command("fox"), Summary("Get an image of a random mischievous fox.")]
        public async Task FoxAsync()
        {
            var fox = JToken.Parse(await Context.HttpClient.GetStringAsync("https://randomfox.ca/floof/"));
            var embed = GetEmbed(Paint.Aqua)
                .WithAuthor("Here's a random fox!", url: fox["link"].ToString())
                .WithImageUrl(fox["image"].ToString())
                .WithFooter("Powered by: randomfox.ca")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("neko"), Summary("The best command out there.")]
        public async Task NekoAsync()
        {
            var embed = GetEmbed(Paint.Aqua)
                .WithImageUrl(JToken.Parse(await Context.HttpClient.GetStringAsync("https://nekos.life/api/v2/img/neko").ConfigureAwait(false))["url"].ToString())
                .WithFooter("Powered by: nekos.life")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }

        [Command("owo")]
        public async Task OwoAsync([Remainder] string message)
        {
            var data = JToken.Parse(await Context.HttpClient.GetStringAsync($"https://nekos.life/api/v2/owoify?text={Uri.EscapeDataString(message)}").ConfigureAwait(false))["owo"];
            await ReplyAsync(data.ToString());
        }

        // TODO: more commands
    }
}
