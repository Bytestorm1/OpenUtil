using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenUtil.Mongo
{
    static class MongoUtil
    {
        private static IMongoCollection<T> GetOrCreateCollection<T>(string t) {
            IMongoCollection<T> output;
            try
            {
                output = Backbone.DB.GetCollection<T>(t);
            }
            catch {
                Backbone.DB.CreateCollection(t);
                output = Backbone.DB.GetCollection<T>(t);
            }
            //I would have this run recursively but I'm not entirely sure how these methods work, so I don't want to risk it crating a bunch of collections
            return output;
        }
        public static IFindFluent<guildData, guildData> findGuildData(ulong id) {
            IMongoCollection<guildData> guildData = GetOrCreateCollection<guildData>("guildData");
            IFindFluent<guildData, guildData> find = guildData.Find(c => c.id == id);
            return find;
        }
        public static guildData getGuildData(ulong id, bool create = true) {
            IFindFluent<guildData, guildData> find = findGuildData(id);
            if (find.CountDocuments() < 1)
            {
                if (create) {
                    //Technically I should have the new object stored separately, but this way I save a bit of processing power
                    //Everything counts
                    addGuildData(new guildData());
                    return new guildData();
                }
                else
                {
                    throw new Exception("Guild data not found");
                }
            }
            else {
                return find.First();
            }
        }
        public static void addGuildData(guildData d) {
            IMongoCollection<guildData> guildData = GetOrCreateCollection<guildData>("guildData");
            guildData.InsertOne(d);
        }
        public static void updateGuildData(guildData newData) {
            IMongoCollection<guildData> guildData = GetOrCreateCollection<guildData>("guildData");
            guildData.FindOneAndReplace(c => c.id == newData.id, newData);
        }
    }
}
