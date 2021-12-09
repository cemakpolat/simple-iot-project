using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrudAPI.Domain
{
    public class TodoItem
    {
        public string Description { get; set; }

        public bool Completed { get; set; }
    }
}