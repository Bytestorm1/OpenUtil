using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
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

            //Status message; keep this set to the repo link
            await client.SetGameAsync("https://github.com/Bytestorm1/OpenUtil");

            client.Ready += ClientReady;            

            //Put stuff here

            //client.[Event] += Event
            client.UserJoined += UserJoined;

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task UserJoined(SocketGuildUser user)
        {
            #region Autorole
            string targetPath = currentPath + $@"/autorole/{user.Guild.Id}.txt";
            //Look for role ID in /autorole/{guild id}.txt
            if (File.Exists(targetPath)) {
                string idString = File.ReadAllLines(targetPath)[0];
                ulong id;
                if (idString == null || idString == "" || !ulong.TryParse(idString, out id))
                {
                    //No autorole assigned
                    return Task.CompletedTask;
                }
                else {
                    user.AddRoleAsync(user.Guild.GetRole(id));
                }
            }
            return Task.CompletedTask;
            #endregion
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
