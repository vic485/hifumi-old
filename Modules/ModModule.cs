using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hifumi.Addons;
using Hifumi.Addons.Preconditions;
using Hifumi.Enums;
using Hifumi.Helpers;
using Hifumi.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Hifumi.Modules
{
    [Name("Moderator Commands"), RequirePermission(AccessLevel.Moderator), RequireBotPermission(ChannelPermission.SendMessages)]
    public class ModModule : Base
    {
        [Command("ban"), Summary("Bans a user from the server."), RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user == await Context.Guild.GetCurrentUserAsync()) return;
            await Context.Guild.AddBanAsync(user, 1, reason).ConfigureAwait(false);
            await Context.GuildHelper.LogAsync(Context, user, CaseType.Ban, reason).ConfigureAwait(false);
            await ReplyAsync($"***{user} was banned.***", document: DocumentType.Server).ConfigureAwait(false);
        }

        [Command("ban"), Summary("Bans multiple users at once."), RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(params IGuildUser[] users)
        {
            foreach (var user in users)
            {
                if (user == await Context.Guild.GetCurrentUserAsync()) continue;
                await Context.Guild.AddBanAsync(user, 1, "Mass ban.");
                await Context.GuildHelper.LogAsync(Context, user, CaseType.Ban, "Mass ban.");
            }
            await ReplyAsync($"{string.Join(", ", users.Select(x => $"*{x.Username}*"))} was banned.", document: DocumentType.Server).ConfigureAwait(false);
        }

        [Command("ban"), Summary("Bans a user from the server."), RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(ulong userId, [Remainder] string reason = null)
        {
            if (userId == (await Context.Guild.GetCurrentUserAsync()).Id) return;
            await Context.Guild.AddBanAsync(userId, 1, reason);
            await ReplyAsync($"*{userId} was banned.*");
        }

        [Command("blacklist"), Summary("Prevent a user from using Hifumi's features in your guild.")]
        public Task Blacklist(AdminCollectionAction action, IGuildUser user)
        {
            if (user.IsBot) return Task.CompletedTask;
            var profile = Context.GuildHelper.GetProfile(Context.Guild.Id, user.Id);
            switch (action)
            {
                case AdminCollectionAction.Add:
                    if (profile.IsBlacklisted) return ReplyAsync($"{user} is already blacklisted.");
                    profile.IsBlacklisted = true;
                    Context.GuildHelper.SaveProfile(Context.Guild.Id, user.Id, profile);
                    return ReplyAsync($"{user} has been blacklisted.");
                case AdminCollectionAction.Remove:
                    if (!profile.IsBlacklisted) return ReplyAsync($"{user} isn't blacklisted.");
                    profile.IsBlacklisted = false;
                    Context.GuildHelper.SaveProfile(Context.Guild.Id, user.Id, profile);
                    return ReplyAsync($"{user} has been whitelisted.");
            }
            return Task.CompletedTask;
        }

        [Command("kick"), Summary("Kicks a user out of the server."), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user == await Context.Guild.GetCurrentUserAsync()) return;
            await user.KickAsync(reason).ConfigureAwait(false);
            await Context.GuildHelper.LogAsync(Context, user, CaseType.Kick, reason).ConfigureAwait(false);
            await ReplyAsync($"***{user} was kicked.***", document: DocumentType.Server).ConfigureAwait(false);
        }

        [Command("kick"), Summary("Kicks multiple users at once."), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickAsync(params IGuildUser[] users)
        {
            foreach (var user in users)
            {
                if (user == await Context.Guild.GetCurrentUserAsync()) continue;
                await user.KickAsync("Mass kick.").ConfigureAwait(false);
                await Context.GuildHelper.LogAsync(Context, user, CaseType.Kick, "Mass kick.").ConfigureAwait(false);
            }
            await ReplyAsync($"{string.Join(", ", users.Select(x => $"*{x.Username}*"))} were kicked.", document: DocumentType.Server).ConfigureAwait(false);
        }

        [Command("mute"), Summary("Mutes a user."), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task MuteAsync(IGuildUser user) // TODO: clean up
        {
            if (user == await Context.Guild.GetCurrentUserAsync()) return;
            if (user.RoleIds.Contains(Context.Server.Mod.MuteRole))
            {
                await ReplyAsync($"{user} is already muted.");
                return;
            }
            if (Context.Guild.Roles.Contains(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted")))
            {
                Context.Server.Mod.MuteRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted").Id;
                await user.AddRoleAsync(Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted"));
                await ReplyAsync($"{user} has been muted.", document: DocumentType.Server);
                Context.Server.Mod.MutedUsers.Add(user.Id);
                return;
            }
            OverwritePermissions permissions = new OverwritePermissions(addReactions: PermValue.Deny, sendMessages: PermValue.Deny, attachFiles: PermValue.Deny);
            if (Context.Guild.GetRole(Context.Server.Mod.MuteRole) == null)
            {
                var role = await Context.Guild.CreateRoleAsync("Muted", GuildPermissions.None, Color.DarkerGrey);
                foreach (var channel in (Context.Guild as SocketGuild).TextChannels)
                    if (!channel.PermissionOverwrites.Select(x => x.Permissions).Contains(permissions))
                        await channel.AddPermissionOverwriteAsync(role, permissions).ConfigureAwait(false);
                Context.Server.Mod.MuteRole = role.Id;
                await user.AddRoleAsync(role);
                await ReplyAsync($"{user} has been muted.", document: DocumentType.Server);
                return;
            }
        }

        [Command("purge"), Priority(0), Summary("Deletes messages from a channel. Default is 10"), RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(int amount = 10)
        {
            var getMessages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await Context.GuildHelper.Purge(getMessages.Cast<IUserMessage>(), Context.Channel as ITextChannel, amount).ConfigureAwait(false);
        }

        [Command("purge"), Priority(10), Summary("Deletes messages from a specified user. Default user is Hifumi, and default amount is 10."), RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeAsync(SocketGuildUser user = null, int amount = 10)
        {
            user = user ?? await Context.Guild.GetCurrentUserAsync() as SocketGuildUser;
            var getMessages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();
            await Context.GuildHelper.Purge(getMessages.Where(x => x.Author.Id == user.Id).Cast<IUserMessage>(), Context.Channel as ITextChannel, amount);
        }

        [Command("reason"), Summary("Specify or change the reason of a mod log case.")]
        public async Task ReasonAsync(int caseNum, [Remainder] string reason)
        {
            var modCase = Context.Server.Mod.Cases.FirstOrDefault(x => x.CaseNumber == caseNum);
            if (modCase == null)
            {
                await ReplyAsync("Invalid case number.");
                return;
            }
            modCase.Reason = reason;
            var channel = await Context.Guild.GetTextChannelAsync(Context.Server.Mod.TextChannel);
            if (await channel.GetMessageAsync(modCase.MessageId) is IUserMessage message)
                await message.ModifyAsync(x => // TODO: embeded message
                {
                    x.Content = $"**{modCase.CaseType}** | Case {modCase.CaseNumber}\n**User:** {StringHelper.CheckUser(Context.Client, modCase.UserId)} ({modCase.UserId})\n" +
                        $"**Reason:** {reason}\n**Responsible Moderator:** {StringHelper.CheckUser(Context.Client, modCase.ModId)}";
                });
            await ReplyAsync($"Case #{caseNum} has been updated.", document: DocumentType.Server);
        }

        [Command("resetwarns"), Summary("Resets a user's warnings.")]
        public Task ResetWarns(IGuildUser user)
        {
            if (user.IsBot) return Task.CompletedTask;
            var profile = Context.GuildHelper.GetProfile(Context.Guild.Id, user.Id);
            profile.Warnings = 0;
            Context.GuildHelper.SaveProfile(Context.Guild.DefaultChannelId, user.Id, profile);
            return ReplyAsync($"{user.Username}'s warnings have been reset.");
        }

        [Command("unmute"), Summary("Unmutes a user."), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnmuteAsync(SocketGuildUser user)
        {
            if (user == await Context.Guild.GetCurrentUserAsync() as SocketGuildUser) return;
            var role = Context.Guild.GetRole(Context.Server.Mod.MuteRole) ?? Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
            if (!user.Roles.Contains(role))
            {
                await ReplyAsync($"{user} doesn't appear to be muted.");
                return;
            }
            await user.RemoveRoleAsync(role);
            Context.Server.Mod.MutedUsers.Remove(user.Id);
            await ReplyAsync($"{user} has been unmuted.");
        }

        [Command("warn"), Summary("Gives a user a warning"), RequireBotPermission(GuildPermission.KickMembers)]
        public async Task WarnAsync(IGuildUser user, [Remainder] string reason = null)
        {
            if (user.IsBot) return;
            string warnmessage = $"[Warned in {Context.Guild.Name}]** {reason}";
            await (await user.GetOrCreateDMChannelAsync()).SendMessageAsync(warnmessage);
            var profile = Context.GuildHelper.GetProfile(Context.Guild.Id, user.Id);
            profile.Warnings++;
            if (Context.Server.Mod.MaxWarnings != 0 && profile.Warnings >= Context.Server.Mod.MaxWarnings)
            {
                await user.KickAsync($"{user} was kicked due to maxing out warnings.");
                await Context.GuildHelper.LogAsync(Context, user, CaseType.Kick, reason);
            }
            else
                await Context.GuildHelper.LogAsync(Context, user, CaseType.Warning, reason);

            Context.GuildHelper.SaveProfile(Context.Guild.Id, user.Id, profile);
            await ReplyAsync($"{user} has been warned.");
        }
    }
}
