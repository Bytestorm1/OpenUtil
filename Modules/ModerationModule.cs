﻿using Discord.Commands;
using Discord.WebSocket;
using OpenUtil.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil.Modules
{
    class ModerationModule : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        [Alias(new string[] { "infraction" })]
        [Summary("Add a warning to a user and show the amount of warnings")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task warnUser(SocketUser user) {
            guildData data = MongoUtil.getGuildData(Context.Guild.Id);

            if (!data.manualModEnabled) {
                Context.Channel.SendMessageAsync("Please enable the moderation module to use this command.");
                return Task.CompletedTask;
            }
            if (data.warnings.ContainsKey(user.Id))
            {
                data.warnings[user.Id]++;
            }
            else {
                data.warnings.Add(user.Id, 1);
            }
            MongoUtil.updateGuildData(data);
            Context.Channel.SendMessageAsync($"Warned user {user.Username}#{user.Discriminator}.\nThey now have {data.warnings[user.Id]} warnings.");
            return Task.CompletedTask;
        }
        [Command("warnings")]
        [Alias(new string[] { "warns", "infractions" })]
        [Summary("Get the amount of times a user has been warned")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task userWarns(SocketUser user) {
            guildData data = MongoUtil.getGuildData(Context.Guild.Id);
            if (!data.manualModEnabled)
            {
                Context.Channel.SendMessageAsync("Please enable the moderation module to use this command.");
                return Task.CompletedTask;
            }
            if (data.warnings.ContainsKey(user.Id))
            {
                Context.Channel.SendMessageAsync($"User {user.Username}#{user.Discriminator} has {data.warnings[user.Id]} warnings");
            }
            else
            {
                Context.Channel.SendMessageAsync($"User {user.Username}#{user.Discriminator} has no warnings.");
            }            
            return Task.CompletedTask;
        }
        [Command("clear-warnings")]
        [Alias("cwarnings", "clear-warns", "clear-infractions", "cwarns", "cinfractions")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task clearWarnings(SocketUser user) {
            guildData data = MongoUtil.getGuildData(Context.Guild.Id);

            if (!data.manualModEnabled)
            {
                Context.Channel.SendMessageAsync("Please enable the moderation module to use this command.");
                return Task.CompletedTask;
            }
            if (data.warnings.ContainsKey(user.Id))
            {
                data.warnings.Remove(user.Id);
            }
            MongoUtil.updateGuildData(data);
            Context.Channel.SendMessageAsync($"Cleared user {user.Username}#{user.Discriminator} of all warnings/infractions.");
            return Task.CompletedTask;
        }
        [Command("mute")]
        [Summary("Stop a user from talking")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task muteUser(SocketUser user) {
            guildData data = MongoUtil.getGuildData(Context.Guild.Id);
            if (!data.manualModEnabled)
            {
                Context.Channel.SendMessageAsync("Please enable the moderation module to use this command.");
                return Task.CompletedTask;
            }
            if (data.mutedRole == 0) {
                Context.Channel.SendMessageAsync($"Muted role not set! Set it with {data.prefix}set-mutedrole");
                return Task.CompletedTask;
            }
            SocketGuildUser sg = user as SocketGuildUser;
            if (sg == null) {
                Context.Channel.SendMessageAsync("This command can only work in servers");
                return Task.CompletedTask;
            }
            sg.AddRoleAsync(Context.Guild.GetRole(data.mutedRole));
            return Task.CompletedTask;
        }
        [Command("unmute")]
        [Summary("Allow a user to talk again")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]
        public Task unmuteUser(SocketUser user)
        {
            guildData data = MongoUtil.getGuildData(Context.Guild.Id);
            if (!data.manualModEnabled)
            {
                Context.Channel.SendMessageAsync("Please enable the moderation module to use this command.");
                return Task.CompletedTask;
            }
            if (data.mutedRole == 0)
            {
                Context.Channel.SendMessageAsync($"Muted role not set! Set it with {data.prefix}set-mutedrole");
                return Task.CompletedTask;
            }
            SocketGuildUser sg = user as SocketGuildUser;
            if (sg == null)
            {
                Context.Channel.SendMessageAsync("This command can only work in servers");
                return Task.CompletedTask;
            }
            sg.RemoveRoleAsync(Context.Guild.GetRole(data.mutedRole));
            return Task.CompletedTask;
        }
    }
}
