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
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task resetCmdPrefix() {
            return setCmdPrefix("u-");
        }
        [Command("autorole")]
        [RequireUserPermission(Discord.GuildPermission.ManageRoles)]
        public Task setAutorole([Summary("Id of the role to automatically assign. If left blank it will disable autorole")]ulong roleId = 0, 
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
        public Task blacklistAdd(params string[] words) {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);

            d.blacklistedWords.AddRange(words);
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
        [Command("blacklist-remove")]
        public Task blacklistRemove(params string[] words)
        {
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            foreach (string word in words) {
                d.blacklistedWords.Remove(word);
            }
            MongoUtil.updateGuildData(d);
            return Task.CompletedTask;
        }
    }
}
