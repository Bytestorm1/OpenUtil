using Discord;
using Discord.Commands;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenUtil.Modules
{
    class PollModule : ModuleBase<SocketCommandContext>
    {
        public static string[] pollEmojis = new string[] { "1️⃣", "2️⃣", "3️⃣", "4️⃣", "5️⃣", "6️⃣", "7️⃣", "8️⃣", "9️⃣", "🇦", "🇧", "🇨", "🇩", "🇪", "🇫", "🇬", "🇭", "🇮", "🇯", "🇰", "🇱", "🇲", "🇳", "🇴", "🇵", "🇶", "🇷", "🇸", "🇹", "🇺", "🇻", "🇼", "🇽", "🇾", "🇿" };
        public static char[] pollLetters = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        [Command("poll")]
        public async Task poll(string poll, params string[] options) {
            EmbedBuilder eb = new EmbedBuilder();
            string optionstring = "";
            for (int i = 0; i < options.Length && i < pollEmojis.Length; i++) {
                //Emoji current = new Emoji(pollEmojis[i].ToString());
                optionstring += $"{pollLetters[i]} - {options[i]}\n";
            }
            eb.AddField(poll, optionstring);
            RestUserMessage msg = await Context.Channel.SendMessageAsync("", false, eb.Build());
            for (int i = 0; i < options.Length && i < pollEmojis.Length; i++) {
                string e = pollEmojis[i];
                Emoji current = new Emoji(e);
                await msg.AddReactionAsync(current);
            }
            return;
        }
    }
}
