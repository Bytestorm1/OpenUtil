using Discord;
using Discord.Commands;
using OpenUtil.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil.Modules
{
    class SettingsModule : ModuleBase<SocketCommandContext>
    {
        [Command("setprefix")]
        [Summary("Set the prefix that should be used for this bot's commands")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task setCmdPrefix(string p) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);

            d.prefix = p;
            MongoUtil.updateGuildData(d);
            Context.Guild.CurrentUser.ModifyAsync(c => c.Nickname = $"OpenUtil ({p})");
            Context.Channel.SendMessageAsync($"Set prefix to {p}");
            return Task.CompletedTask;
        }
        [Command("resetprefix")]
        [Summary("Reset the bot's prefix for the current server")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task resetCmdPrefix() {
            return setCmdPrefix("u-");
        }
        [Command("set-autorole")]
        [Summary("Set or disable the autorole feature. To set, input a role id and optionally, a value for whether or not to maintain the role.\nTo disable autorole, simply leave all parameters blank")]
        [RequireUserPermission(Discord.GuildPermission.ManageRoles)]
        public Task setAutorole(
            [Summary("Id of the role to automatically assign. If left blank it will disable autorole")]ulong roleId = 0, 
            [Summary("Whether the bot should make sure all users have the role, or only give it once")]bool maintain = false) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);

            if (roleId == 0) {
                d.autoRole = null;
                Context.Channel.SendMessageAsync("Autorole disabled");
                return Task.CompletedTask;
            }
            d.autoRole = Context.Guild.GetRole(roleId);
            d.maintainRole = maintain;
            MongoUtil.updateGuildData(d);
            Context.Channel.SendMessageAsync($"Set autorole to **{d.autoRole.Name}**");
            return Task.CompletedTask;
        }
        [Command("autorole")]
        [Summary("Returns what role is currently being assigned automatically, or if no autorole is currently set.")]
        public Task getAutorole() {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);

            if (d.autoRole != null)
            {
                Context.Channel.SendMessageAsync($"Autorole is currently set to: **{d.autoRole.Name}**");
            }
            else {
                Context.Channel.SendMessageAsync("Autorole is currently not enabled on this server");
            }
            return Task.CompletedTask;
        }

        //CUSTOM ROLE COMMANDS IN RoleModule.cs

        [Command("set-automod")]
        [Summary("Enable or disable automod")]
        public Task setAutomod(bool enable) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);

            d.automodEnabled = enable;
            MongoUtil.updateGuildData(d);
            if (enable) {
                Context.Channel.SendMessageAsync($"Automod enabled.\n" +
                    $"Add/Remove blacklisted words with `{d.prefix}blacklist-add` and `{d.prefix}blacklist-remove`\n" +
                    $"Add/Remove channels that automod should ignore with `{d.prefix}ignorech-add` and `{d.prefix}ignorech-remove`");
            }
            else
            {
                Context.Channel.SendMessageAsync("Automod disabled.");
            }
            return Task.CompletedTask;
        }
        [Command("blacklist-add")]
        [Summary("Add words to the automod blacklist")]
        public Task blacklistAdd(params string[] words) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            d.blacklistedWords.AddRange(words);
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
        [Command("blacklist-remove")]
        [Summary("Remove words from the automod blacklist")]
        public Task blacklistRemove(params string[] words)
        {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            foreach (string word in words)
            {
                d.blacklistedWords.Remove(word);
            }
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
        [Command("ignorech-add")]
        [Summary("Add words to the automod blacklist")]
        public Task ignorechAdd(IMessageChannel channel = null)
        {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            if (channel == null)
            {
                d.ignoredChannelIds.Add(Context.Channel.Id);
            }
            else {                
                d.ignoredChannelIds.Add(channel.Id);
            }
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
        [Command("ignorech-remove")]
        [Summary("Remove channels from the automod ignored channels")]
        public Task ignorechRemove(IMessageChannel channel = null)
        {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            if (channel == null)
            {
                d.ignoredChannelIds.Remove(Context.Channel.Id);
            }
            else
            {
                //No need to check validity
                d.ignoredChannelIds.Remove(channel.Id);
            }
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
        [Command("set-mutedrole")]
        [Summary("Set the role assigned to muted users. Leave this blank to remove the setting.")]
        public Task setMutedRole(ulong RoleId = 0) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            if (Context.Guild.GetRole(RoleId) == null) {
                Context.Channel.SendMessageAsync("Role not found.");
                return Task.CompletedTask;
            }
            d.mutedRole = RoleId;
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
    }
}
