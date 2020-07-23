using Discord;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenUtil.Mongo
{
    class guildData : IMongoWrapper
    {
        //Server id so it can be found
        public guildData(ulong id) {
            this.id = id;
        }
        public guildData()
        {
            //Values are already set to their defaults, this is just here to ensure Mongo isn't weird
        }
        public ulong id;
        public string prefix = "u-";
        public void registerClassMap() {
            BsonClassMap m = BsonClassMap.RegisterClassMap<guildData>(cm => {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.id));
            });
        }
        #region Autorole
        //Leave null to disable
        public IRole autoRole;
        //whether or not the bot should ensure that all users have this role
        public bool maintainRole = false;
        #endregion
        #region Role commands
        public Dictionary<string, IRole> roleCmds = new Dictionary<string, IRole>();
        #endregion
        #region Automod
        public bool automodEnabled = false;
        public List<string> blacklistedWords = new List<string>();
        public List<ulong> ignoredChannelIds = new List<ulong>();
        public bool illegalWord(string word, int depth = 1)
        {
            //For those adapting to their own server: 
            //I reccomend changing this method up a bit so people can't look up the code and find some weaknesses

            if (blacklistedWords.Contains(word)) { return true; }
            //Check for censored vowels
            //char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            char[] censors = new char[] { '*', '-', '/', '#', '@', '!', '^', '~'};
            //Check for general censored characters; check for up to depth characters censored
            char[] wordChars = word.ToCharArray();
            for (int i = 0; i < word.Length; i++) {
                if (censors.Contains(wordChars[i])) {
                    string c = word.Substring(0, i) + word.Substring(i + 1, word.Length);                    
                    foreach (string b in blacklistedWords) {
                        string rem = b.Substring(0, i) + b.Substring(i + 1, word.Length);
                        if (c.Equals(b)) {
                            return true;
                        }
                    }
                }
            }
            //TODO: Add detection for censoring characters to bypass blacklist
            
            return false;
        }
        public bool illegalMsg(string msg) {
            foreach (string s in msg.Split(' ')) {
                if (illegalWord(s)) {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region ManualMod
        public bool manualModEnabled = true;
        public ulong mutedRole;
        public Dictionary<ulong, int> warnings = new Dictionary<ulong, int>();
        #endregion
        
        //TODO: Live Feeds
    }
}
