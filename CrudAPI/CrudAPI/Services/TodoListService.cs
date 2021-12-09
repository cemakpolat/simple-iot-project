using System.Collections.Generic;
using System.Threading.Tasks;
using CrudAPI.Domain;
using CrudAPI.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CrudAPI.Services
{
    public class TodoListService
    {
        private readonly IMongoCollection<TodoList> _collection;

        public TodoListService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _collection = database.GetCollection<TodoList>("todolists");
        }


        public async Task<TodoList> Insert(TodoList todoList) //Create
        {
            await _collection.InsertOneAsync(todoList);
            return todoList;
        }

        public async Task<TodoList> Update(TodoList todoList) //Update
        {
            await _collection.ReplaceOneAsync(t => t.Id
                                                   == todoList.Id, todoList);
            return todoList;
        }

        public async Task<IList<TodoList>> GetAll() //Read
        {
            return await _collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }


        public async Task<TodoList> GetById(string todoListId) //Read
        {
            return await _collection.FindAsync(t => t.Id == todoListId).Result.FirstOrDefaultAsync();
        }

        public async Task Remove(string todoListId) //Delete
        {
            await _collection.DeleteOneAsync(t => t.Id
                                                  == todoListId);
        }
    }
}