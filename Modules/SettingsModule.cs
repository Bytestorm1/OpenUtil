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
            guildData d = new guildData();
            try
            {
                MongoUtil.getGuildData(Context.Guild.Id);
            }
            catch {
                d.id = Context.Guild.Id;
                MongoUtil.addGuildData(d);
            }
            d.prefix = p;
            Context.Channel.SendMessageAsync($"Set prefix to {p}");
            return Task.CompletedTask;
        }
        [Command("resetprefix")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task resetCmdPrefix() {
            return setCmdPrefix("u-");
        }
    }
}
