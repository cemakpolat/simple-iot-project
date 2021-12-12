using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataStore.Domain
{
    
    public class Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public String Id { get; set; }
    
        public String objectType { get; set; }
        public String uuid { get; set; }
        public String groupId { get; set; }
        public String name { get; set; }
        public List<Property> properties { get; set; }
        
    }
}
