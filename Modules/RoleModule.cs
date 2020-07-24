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
            guildData d = MongoUtil.getGuildData(Context.Guild.Id);
            if (role == null)
            {
                string output = "```";
                foreach (string s in d.roleCmds.Keys.ToList())
                {
                    output += s + "\n";
                }
                output += "```";
                Context.Channel.SendMessageAsync("Here are the current role commands:\n" + output);
            }
            else {
                IRole r;
                SocketGuildUser s = Context.User as SocketGuildUser;
                if (!d.roleCmds.TryGetValue(role, out r))
                {
                    Context.Channel.SendMessageAsync("Role not found.");
                    return Task.CompletedTask;
                }
                if (s.Roles.Contains(r))
                {
                    s.RemoveRoleAsync(r);
                    Context.Channel.SendMessageAsync("Role removed.");
                }
                else
                {
                    s.AddRoleAsync(r);
                    Context.Channel.SendMessageAsync("Role added.");
                }
            }
            return Task.CompletedTask;
        }
        [Command("addrole")]
        [Summary("Add a role command")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public Task addRoleCmd(string cmd, ulong roleId) {
            guildData find = MongoUtil.getGuildData(Context.Guild.Id);
            if (find.roleCmds.ContainsKey(cmd))
            {
                Context.Channel.SendMessageAsync("There is already a role command attatched to that. Please try a different one.");
            }
            else { //Allow multiple commands to point to one role
                IRole r = Context.Guild.GetRole(roleId);
                if (r == null) {
                    Context.Channel.SendMessageAsync("Role not found!");
                    return Task.CompletedTask;
                }
                find.roleCmds.Add(cmd, r);
            }
            MongoUtil.updateGuildData(find);
            return Task.CompletedTask;
        }
    }
}
