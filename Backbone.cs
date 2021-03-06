﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OpenUtil.Mongo;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenUtil
{
    class Backbone
    {
        public static MongoClient mClient = new MongoClient();
        public static IMongoDatabase DB = mClient.GetDatabase("OpenUtil");
        public static string DEFAULT_PREFIX = "u-";
        public static ulong PERMISSIONS_BIT = 342912064;

        static private DiscordSocketClient client;
        static private CommandService Commands;
        static private CommandHandler commandHandler;
        public static string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            new Backbone().MainAsync().GetAwaiter().GetResult();

        }

        private async Task MainAsync()
        {
            //Console.WriteLine(Globals.senatorsToRep(740));
            client = new DiscordSocketClient();
            Commands = new CommandService();
            commandHandler = new CommandHandler(client, Commands);

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug //Set to Error or Critical at Release
            });
            Console.WriteLine("Logging In...");
            await client.LoginAsync(TokenType.Bot, Tokens.token_bot);
            await client.StartAsync();
            Console.WriteLine("Logged In");

            //mClient = new MongoClient();
            //DB = mClient.GetDatabase("OpenUtil");

            //This handles incoming messages and commands
            await commandHandler.InstallCommandsAsync();
            client.MessageReceived += commandHandler.HandleCommandAsync;

            //Status message; keep this set to the repo link
            await client.SetGameAsync("github.com/Bytestorm1/OpenUtil");

            client.Ready += ClientReady;            

            //Put stuff here

            //client.[Event] += Event
            client.UserJoined += UserJoined;
            client.UserLeft += UserLeft;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task UserLeft(SocketGuildUser user)
        {
            #region Leave message
            guildData d = MongoUtil.getGuildData(user.Guild.Id);
            SocketGuild g = client.GetGuild(user.Guild.Id);
            g.SystemChannel.SendMessageAsync(d.joinMsg);
            #endregion
            return Task.CompletedTask;
        }

        private Task UserJoined(SocketGuildUser user)
        {
            #region Autorole
            guildData d = MongoUtil.getGuildData(user.Guild.Id);
            if (d.autoRole != null) {
                user.AddRoleAsync(d.autoRole);
            }
            #endregion
            #region Join message
            SocketGuild g = client.GetGuild(user.Guild.Id);
            g.SystemChannel.SendMessageAsync(d.joinMsg);
            #endregion
            return Task.CompletedTask;
        }

        //Called from command handler
        public void MessageReceived(SocketMessage msg) {

        }

        private Task ClientReady()
        {
            Console.WriteLine("Ready");
            return Task.CompletedTask;
        }
    }
}
