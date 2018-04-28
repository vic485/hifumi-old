using Discord;
using Discord.Commands;
using Hifumi.Addons;
using static Hifumi.Addons.Embeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Game Commands"), RequireBotPermission(ChannelPermission.SendMessages)]
    public class GamesModule : Base
    {
        [Command("steam"), Summary("Show a user's steam profile.")]
        public async Task SteamAsync(string userId)
        {
            var userInfo = await Context.ConfigHandler.HGame.Steam.GetUsersInfoAsync(new[] { userId }.ToList());
            var userGames = await Context.ConfigHandler.HGame.Steam.OwnedGamesAsync(userId);
            var userRecent = await Context.ConfigHandler.HGame.Steam.RecentGamesAsync(userId);
            var info = userInfo.PlayersInfo.Players.FirstOrDefault();
            string state;
            if (info.ProfileState == 0) state = "Offline";
            else if (info.ProfileState == 1) state = "Online";
            else if (info.ProfileState == 2) state = "Busy";
            else if (info.ProfileState == 3) state = "Away";
            else if (info.ProfileState == 4) state = "Snooze";
            else if (info.ProfileState == 5) state = "Looking to trade";
            else state = "Looking to play";

            var embed = GetEmbed(Paint.Aqua)
                .WithAuthor(info.RealName ?? "Steam", "https://png.icons8.com/material/256/e5e5e5/steam.png", info.ProfileLink)
                .WithThumbnailUrl(info.AvatarFullUrl)
                .AddField("Display Name", $"{info.Name}", true)
                .AddField("Location", $"{info.State ?? "No State"}, {info.Country ?? "No Country"}", true)
                .AddField("Person State", state, true)
                .AddField("Profile Created", Context.MethodHelper.UnixDateTime(info.TimeCreated), true)
                .AddField("Last Online", Context.MethodHelper.UnixDateTime(info.LastLogOff), true)
                .AddField("Primary clan ID", info.PrimaryClanId ?? "None", true)
                .AddField("Owned Games", userGames.OwnedGames.GamesCount, true)
                .AddField("Recently Played Games", userRecent.RecentGames.TotalCount, true);
            await ReplyAsync(string.Empty, embed.Build());
        }

        [Command("wows"), Summary("Show a user profile for World of Warships.")]
        public async Task WowsAsync() // TODO: image sharp
        {
            HGame.Wows.Region region;
            List<string> usernames = new List<string>();
            var msg = await ReplyAsync("Type the number of a region below:\n" +
                "```css\n" +
                "[1] Russia\n" +
                "[2] Europe\n" +
                "[3] North America\n" +
                "[4] Asia```");
            var temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp != null)
            {
                int.TryParse(temp.Content, out int i);
                if (i > 0 && i < 5) region = (HGame.Wows.Region)(i - 1);
                else
                {
                    await ReplyAsync("Invalid region");
                    return;
                }
            }
            else
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }
            msg = await ReplyAsync("Type the username of a player");
            temp = await ResponseWaitAsync(timeout: TimeSpan.FromSeconds(15));
            await msg.DeleteAsync();
            if (temp != null)
            {
                usernames.Add(temp.Content);
            }
            else
            {
                await ReplyAsync($"**{Context.User.Username}**, the window has closed due to inactivity.");
                return;
            }
            var user = await Context.ConfigHandler.HGame.Wows.GetPlayersAsync(region, usernames);
            if (user == null)
            {
                await ReplyAsync($"Couldn't find a user matching {usernames[0]}");
                return;
            }
            List<string> ids = new List<string>(new string[] { user.PlayerList[0].AccountId.ToString() });
            var data = await Context.ConfigHandler.HGame.Wows.GetPlayerDataAsync(region, ids);
            var embed = GetEmbed(Paint.Aqua)
                .WithTitle($"{user.PlayerList[0].Nickname}'s Profile")
                .AddField("Wins", data[0].Statistics.PVP.Wins, true)
                .AddField("Losses", data[0].Statistics.PVP.Losses, true)
                .AddField("Total Battles", data[0].Statistics.Battles, true)
                .AddField("XP", data[0].Statistics.PVP.XP)
                .AddField("Distance travelled", $"{data[0].Statistics.Distance} miles", true)
                .AddField("Main battery hits", data[0].Statistics.PVP.MainBattery.Hits, true)
                .AddField("Main battery shots", data[0].Statistics.PVP.MainBattery.Shots, true)
                .AddField("Warships destroyed with Main Battery", data[0].Statistics.PVP.MainBattery.Frags, true)
                .WithFooter($"Last Online: {Context.MethodHelper.UnixDateTime(data[0].LogoutAt).ToString()}")
                .Build();
            await ReplyAsync(string.Empty, embed);
        }
    }
}
