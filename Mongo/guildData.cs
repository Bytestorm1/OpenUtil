using Discord;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenUtil.Mongo
{
    class guildData : IMongoWrapper
    {
        //Server id so it can be found
        public ulong id;
        public void registerClassMap() {
            BsonClassMap m = BsonClassMap.RegisterClassMap<guildData>(cm => {
                cm.AutoMap();
                cm.SetIdMember(cm.GetMemberMap(c => c.id));
            });
        }
        #region Autorole
        public bool autoRoleEnabled = false;
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
        public bool illegalWord(string word)
        {
            if (blacklistedWords.Contains(word)) { return true; }
            //char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };

            //TODO: Add detection for censoring characters to bypass blacklist
            
            return false;
        }
        #endregion
        #region ManualMod
        public bool manualModEnabled = true;
        public ulong mutedRole;
        public Dictionary<ulong, int> warnings = new Dictionary<ulong, int>();
        #endregion
    }
}
