using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenUtil.Mongo
{
    static class MongoUtil
    {
        public static IFindFluent<guildData, guildData> findGuildData(ulong id) {
            IMongoCollection<guildData> guildData = Backbone.DB.GetCollection<guildData>("guildData");
            IFindFluent<guildData, guildData> find = guildData.Find(c => c.id == id);
            return find;
        }
        public static guildData getGuildData(ulong id) {
            IFindFluent<guildData, guildData> find = findGuildData(id);
            if (find.CountDocuments() < 1)
            {
                throw new Exception("Guild data not found");
            }
            else {
                return find.First();
            }
        }
        public static void addGuildData(guildData d) {
            IMongoCollection<guildData> guildData = Backbone.DB.GetCollection<guildData>("guildData");
            guildData.InsertOne(d);
        }
    }
}
