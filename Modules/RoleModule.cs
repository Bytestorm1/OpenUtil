using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Driver;
using OpenUtil.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil.Modules
{
    class RoleModule : ModuleBase<SocketCommandContext>
    {
        [Command("role")]
        [Summary("Assign a role to the user or list the current roles applicable")]
        public Task roleAssign(string role = null) {
            IFindFluent<guildData, guildData> find = MongoUtil.findGuildData(Context.Guild.Id);
            if (role == null && find.CountDocuments() > 0)
            {
                string output = "```";
                foreach (string s in find.First().roleCmds.Keys.ToList())
                {
                    output += s + "\n";
                }
                output += "```";
                Context.Channel.SendMessageAsync("Here are the current role commands:\n" + output);
            }
            else if (find.CountDocuments() > 0) {
                IRole r;
                SocketGuildUser s = Context.User as SocketGuildUser;
                if (!find.First().roleCmds.TryGetValue(role, out r)) {
                    Context.Channel.SendMessageAsync("Role not found.");
                    return Task.CompletedTask;
                }                
                s.AddRoleAsync(r);
            }
            else
            {
                Context.Channel.SendMessageAsync("No role commands found!");
            }
            return Task.CompletedTask;
        }

    }
}
