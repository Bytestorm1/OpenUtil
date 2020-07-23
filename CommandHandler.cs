using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OpenUtil.Modules;
using OpenUtil.Mongo;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil
{
    //From https://discord.foxbot.me/docs/guides/commands/intro.html
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModuleAsync<HelpModule>(null);
            await _commands.AddModuleAsync<ModerationModule>(null);
            await _commands.AddModuleAsync<RoleModule>(null);
            await _commands.AddModuleAsync<SettingsModule>(null);
            await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            guildData d = null;
            try
            {
                d = MongoUtil.getGuildData(context.Guild.Id);
            }
            catch {
                //Move on
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = Backbone.DEFAULT_PREFIX.Length;
            string p = Backbone.DEFAULT_PREFIX;
            if (d != null) {
                //Automod
                /**
                 * Check if Automod is enabled, 
                 * the current channel is not meant to be ignored, 
                 * and then if the message is blacklisted
                 */
                if (d.automodEnabled && 
                    !d.ignoredChannelIds.Contains(context.Channel.Id) && 
                    d.illegalMsg(message.Content)) {
                    await message.DeleteAsync();
                    return;
                }
                //Command
                argPos = d.prefix.Length;
                p = d.prefix;
            }

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(p, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;
            else {
                Console.WriteLine($"Command Received: {message.Content}");
            }
            

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.

            // Keep in mind that result does not indicate a return value
            // rather an object stating if the command executed successfully.
            var result = await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            // Optionally, we may inform the user if the command fails
            // to be executed; however, this may not always be desired,
            // as it may clog up the request queue should a user spam a
            // command.
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync($@"An error ocurred: {result.ErrorReason}\nPlease report this at `https://github.com/Bytestorm1/OpenUtil/issues` with screenshots if possible.");
            }
        }
    }
}
