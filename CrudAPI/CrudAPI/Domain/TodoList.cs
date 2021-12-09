using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrudAPI.Domain
{
    public class TodoList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public IList<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
    }
}