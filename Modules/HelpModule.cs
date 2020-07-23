﻿using Discord;
using Discord.Commands;
using OpenUtil.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        public HelpModule(CommandService cmdService)
        {
            _commandService = cmdService;
        }
        [Command("invite")]
        [Summary("Get a link to invite this bot to another server")]
        public async Task invite()
        {
            await Context.Channel.SendMessageAsync($@"https://discordapp.com/api/oauth2/authorize?client_id={Tokens.token_clientID}&permissions={Backbone.PERMISSIONS_BIT}&scope=bot");
        }

        [Command("supportserver")]
        [Summary("Get a link to join the support server")]
        public async Task supportserver()
        {
            await Context.Channel.SendMessageAsync("Join the support server at: \n https://discord.gg/FPguN2g");
        }
        [Command("help")]
        [Summary("Get the list of commands available in this bot, or get the syntax for using a specific command")]
        public async Task help(string commandString = "not_actually_default_ignore_this")
        {
            if (commandString != "not_actually_default_ignore_this")
            {
                await helpCommand(commandString);
                return;
            }
            IReadOnlyCollection<CommandInfo> l = await _commandService.GetExecutableCommandsAsync(Context, null);
            string prefix = Backbone.DEFAULT_PREFIX;
            try
            {
                guildData d = MongoUtil.getGuildData(Context.Guild.Id, false);
                prefix = d.prefix;
            }
            catch {
                prefix = Backbone.DEFAULT_PREFIX;
            }
            List<string> s = new List<string>();
            foreach (CommandInfo cmd in l)
            {
                string e = "**" + prefix + cmd.Name + " ";
                foreach (ParameterInfo p in cmd.Parameters)
                {
                    if (p.DefaultValue != null)
                    {
                        e += "[" + p.Name + "=" + p.DefaultValue + "] ";
                    }
                    else
                    {
                        e += "<" + p.Name + "> ";
                    }
                }
                e += "**";
                if (cmd.Summary != null)
                {
                    e += "\n```" + cmd.Summary + "```";
                }
                s.Add(e);
            }
            //s.Add("If you are seeing this line, send a screennshot of this command to the bot dev.");
            List<string> output = new List<string>();
            //s.ForEach(item => output += item);
            int i = 0;
            foreach (string c in s)
            {
                if (output.Count == 0 || i >= output.Count)
                {
                    output.Add("");
                }
                if ((output[i] + c + "\n").Length > 2000)
                {
                    i++;
                    if (output.Count == 0 || i >= output.Count)
                    {
                        output.Add("");
                    }
                }
                output[i] += c + "\n";
            }
            IDMChannel channel = await Context.User.GetOrCreateDMChannelAsync();
            foreach (string ae in output)
            {
                await channel.SendMessageAsync(">>> " + ae);
            }
            //await Context.Channel.SendMessageAsync(output);
            await Context.Channel.SendMessageAsync("Sent you a DM!");
        }
        private async Task helpCommand(string command)
        {
            IReadOnlyCollection<CommandInfo> l = await _commandService.GetExecutableCommandsAsync(Context, null);
            string prefix = Backbone.DEFAULT_PREFIX;
            try
            {
                guildData d = MongoUtil.getGuildData(Context.Guild.Id, false);
                prefix = d.prefix;
            }
            catch
            {
                prefix = Backbone.DEFAULT_PREFIX;
            }
            CommandInfo cmd = null;
            string CommandString = command;
            if (CommandString.StartsWith(prefix))
            {
                CommandString = CommandString.Substring(3);
            }
            if (l.Where(item => item.Aliases.Contains(CommandString) || item.Name == CommandString).Count() > 0)
            {
                cmd = l.Where(item => item.Aliases.Contains(CommandString) || item.Name == CommandString).First();
            }
            else
            {
                await Context.Channel.SendMessageAsync("Command " + CommandString + " not found.");
                return;
            }

            string e = "**Help for " + prefix + cmd.Name + " ";
            string desc = ">>> ";
            foreach (ParameterInfo p in cmd.Parameters)
            {
                desc += "**" + p.Name + "**\n```Summary: " + p.Summary;
                if (p.DefaultValue != null)
                {
                    e += "[" + p.Name + "=" + p.DefaultValue + "] ";
                    desc += "\nDefaults to: " + p.DefaultValue.ToString();
                }
                else
                {
                    e += "<" + p.Name + "> ";                    
                }
                desc += "\nType: " + p.Type.ToString() + "\n" + "```";
            }
            e += "**";
            await Context.Channel.SendMessageAsync(e + "\n" + desc);
        }
    }
}
