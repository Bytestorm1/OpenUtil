using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenUtil.Mongo
{
    interface IMongoWrapper
    {
        void registerClassMap();
    }
}
