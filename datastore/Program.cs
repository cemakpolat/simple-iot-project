
using System;
using MongoDB.Driver;
using MongoDB.Bson;

namespace datastore
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            var dbList = dbClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases are:");

            foreach (var item in dbList)
            {
                Console.WriteLine(item);
            }
        }
        public void find(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");
            var cars = db.GetCollection<BsonDocument>("cars");

            var filter = Builders<BsonDocument>.Filter.Eq("price", 29000);

            var doc = cars.Find(filter).FirstOrDefault();
            Console.WriteLine(doc.ToString());
        }
        public void findAll(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");
            var documents = cars.Find(new BsonDocument()).ToList();

            foreach (BsonDocument doc in documents)
            {
                Console.WriteLine(doc.ToString());
            }
        }
        public void query(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");
            var cars = db.GetCollection<BsonDocument>("cars");

            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Gt("price", 30000) & builder.Lt("price", 55000);

            var docs = cars.Find(filter).ToList();

            docs.ForEach(doc => {
                Console.WriteLine(doc);
            });
        }
        public void insert(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");

            var doc = new BsonDocument
            {
                {"name", "BMW"},
                {"price", 34621}
            };

            cars.InsertOne(doc);
        }
        public void skipLimit(){
             var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");
            var docs = cars.Find(new BsonDocument()).Skip(3).Limit(3).ToList();

            docs.ForEach(doc =>
            {
                Console.WriteLine(doc);
            });
        }
        public void projections(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");
            var docs = cars.Find(new BsonDocument()).Project("{_id: 0}").ToList();

            docs.ForEach(doc =>
            {
                Console.WriteLine(doc);
            });
        }
        public void delete(){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");
            var filter = Builders<BsonDocument>.Filter.Eq("name", "BMW");

            cars.DeleteOne(filter);
        }

        public void update(){
             var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("testdb");

            var cars = db.GetCollection<BsonDocument>("cars");

            var filter = Builders<BsonDocument>.Filter.Eq("name", "Audi");
            var update = Builders<BsonDocument>.Update.Set("price", 52000);

            cars.UpdateOne(filter, update);
        }
    }
}
