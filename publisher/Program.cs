using System;
using System.Text;

using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using MongoDB.Driver;
using MongoDB.Bson;

namespace publisher
{
    class Program
    {

         static void Main(string[] args)
        {
            DataStore store = new DataStore();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId("Server")
                                        .WithTcpServer("127.0.0.1", 1883);

            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();

            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            // _mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a => {
            //     Log.Logger.Information("Message recieved: {payload}", a.ApplicationMessage);
            //     var user = JsonConvert.DeserializeObject<Object>( a.ApplicationMessage);
            //     Console.WriteLine(user);

            // });
            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
                store.insert(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

                // Task.Run(() => mqttClient.PublishAsync("hello/world"));
            });

            _mqttClient.StartAsync(options).GetAwaiter().GetResult();

            // _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopic("dev.to/topic/json").Build());

           _mqttClient.SubscribeAsync(
             new MqttTopicFilter
                 {
                    Topic = "dev.to/topic/json"
                }
            );
            Console.ReadLine();

        }

        public static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully connected.");
        }

        public static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Log.Logger.Warning("Couldn't connect to broker.");
        }

        public static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully disconnected.");
        }
    }
    class DataStore{

        public DataStore(){}
        public void insert(String measurement){
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("bosch");

            var cars = db.GetCollection<BsonDocument>("sensors");

            var doc = new BsonDocument
            {
                {"measuremetn", measurement}
            };

            cars.InsertOne(doc);
        }
    }
   

}
