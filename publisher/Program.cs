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
using System.Collections;
using System.Collections.Generic;

namespace publisher
{
     public class Property  {
        public String objectType { get; set; }
        
        // Test the following object type
        // public String objectType   
        // {
        //     get { return String; }   
        //     set { String = value; }  
        // }

        public long timestamp { get; set; }
        public Object value { get; set; }
        public String key { get; set; }
        public Property(){}
        public Property(String otype, long tstamp, String key, Object value){
            this.objectType = otype;
            this.timestamp = tstamp;
            this.key = key;
            this.value = value;
        }
    
    }

    class IoTEntity {

        
        public String objectType { get; set; }
        public String uuid { get; set; }
        public String groupId { get; set; }
        public String name { get; set; }
        public List<Property> properties = new List<Property>();
        public IoTEntity(){}

        public IoTEntity(String oType, String uuid,  String name, String groupId, List<Property> properties) {
            this.objectType = oType;
            this.uuid = uuid;
            this.groupId = groupId;
            this.name = name;
            if(properties != null){
                this.properties = properties;
            }
            
        }
        public IoTEntity addProperty(Property item){
            bool exist = false;
            foreach(Property element in this.properties){
                if (element.key == item.key){
                    exist = true;
                    break;
                }
            }
            if (exist == false){
                this.properties.Add(item);
            }
            return this;
        }

    }
    // TODO: Look at this example https://github.com/chkr1011/MQTTnet/blob/master/Tests/MQTTnet.TestApp.NetCore/PublicBrokerTest.cs
    class MQTTConnnection{
        IManagedMqttClient _mqttClient  = null;
        String client = "Service";
        String brokerIp = "127.0.0.1";
        int brokerPort = 1883;
        ManagedMqttClientOptions options = null;
        public MQTTConnnection(){
        
            this.setup(client, brokerIp, brokerPort);
            
        }
        public void setup(String client, String url, int port){
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId(client)
                                        .WithTcpServer(url,port);

            options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            this._mqttClient = new MqttFactory().CreateManagedMqttClient();

            this._mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            this._mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            this._mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            this._mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a => {
                Log.Logger.Information("Message recieved: {payload}", a.ApplicationMessage);
                //     var user = JsonConvert.DeserializeObject<Object>( a.ApplicationMessage);
            });
            

            _mqttClient.UseApplicationMessageReceivedHandler(e => handleMessage(e.ApplicationMessage)
            // {
            //     Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            //     Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            //     Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            //     Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            //     Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            //     Console.WriteLine();
            //     store.insert(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

                
            // }
            );

            _mqttClient.StartAsync(options).GetAwaiter().GetResult();

            // _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopic("dev.to/topic/json").Build());

           
        }
        private void handleMessage(MqttApplicationMessage message){
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {message.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(message.Payload)}");
                Console.WriteLine($"+ QoS = {message.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {message.Retain}");
                Console.WriteLine();

                //store.insert(Encoding.UTF8.GetString(message.Payload));
        }

        public void subscribe(String topic){
            this._mqttClient.SubscribeAsync(
             new MqttTopicFilter
                 {
                    Topic = topic //"dev.to/topic/json"
                }
            );

        }
        
         public void sendMessage(String topic, String json){
             Task.Run(() => this._mqttClient.PublishAsync(topic, json));
            this._mqttClient.PublishAsync(topic, json);
        }
        
        public void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully connected.");
        }

        public  void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Log.Logger.Warning("Couldn't connect to broker.");    
        }
        

        public void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Log.Logger.Information("Successfully disconnected.");
            
        }
    }

    
    class Program
    {

         static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            MQTTConnnection mqtt = new MQTTConnnection();
            
            DataStore store = new DataStore();
           
            Console.ReadLine();

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
                {"measurement", measurement}
            };

            cars.InsertOne(doc);
        }
    }
   

}
