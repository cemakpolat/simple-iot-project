using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DataStore.Domain;
using DataStore.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataStore.Services
{
    public class IoTService
    {
        private readonly IMongoCollection<Entity> _collection;

        public IoTService(String connectionString, String db)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(db);
            _collection = database.GetCollection<Entity>("iotentities");
            if(_collection == null){
                createCollection(database, "iotentities");
                _collection = database.GetCollection<Entity>("iotentities");
            }
        }
        
        private async void createCollection(IMongoDatabase database, String collname){
            await database.CreateCollectionAsync(collname);
        }

        public IoTService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
            _collection = database.GetCollection<Entity>("iotentities");
            if(_collection == null){
                createCollection(database, "iotentities");
                _collection = database.GetCollection<Entity>("iotentities");
            }
        }


        public async Task<Entity> Insert(Entity entity) //Create
        {   
            bool exist  = _collection.Find(t => t.uuid == entity.uuid).Any();

            Console.WriteLine(exist);

            if (exist){
                List<Property> list = entity.properties;
                foreach(Property prop in list){

                    var filter = Builders<Entity>.Filter.And(
                    Builders<Entity>.Filter.Eq("uuid", entity.uuid),
                    Builders<Entity>.Filter.Eq("properties.key", prop.key));
                    var update = Builders<Entity>.Update.Push("properties.$.values", prop.values[0]);
                    await _collection.FindOneAndUpdateAsync<BsonDocument>(filter, update);
                }
            }else{
                await _collection.InsertOneAsync(entity);
            }

            return entity;
        }
  
            public async Task<Entity> Insert(String uuid, Property prop, Value val) 
        {
            var filter = Builders<Entity>.Filter.And(
            Builders<Entity>.Filter.Eq("uuid", uuid),
            Builders<Entity>.Filter.Eq("properties.key", prop.key));
            var update = Builders<Entity>.Update.Push("properties.$.values", val);
            
            return null;
        }
        

        public async Task<Entity> Update(Entity entity) //Update
        {
            await _collection.ReplaceOneAsync(t => t.uuid == entity.uuid, entity);
            return entity;
        }


        public async Task<IList<Entity>> GetAll() //Read
        {
            return await _collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<Entity> GetById(string entityId) //Read
        {
            return await _collection.FindAsync(t => t.Id == entityId).Result.FirstOrDefaultAsync();
        }

        public async Task Remove(string entity) //Delete
        {
            await _collection.DeleteOneAsync(t => t.Id == entity);
        }
        
        
    }
}