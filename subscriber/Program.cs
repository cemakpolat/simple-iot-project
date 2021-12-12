using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using Serilog;
using System.Collections;
using System.Collections.Generic;


namespace subscriber
{   
      
      public class Property  {
        public String objectType { get; set; }

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

    class MQTTConnnection{
        IManagedMqttClient _mqttClient  = null;
        String client = "Sensor";
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
            });
            //                await client.ConnectAsync(options);

            _mqttClient.StartAsync(options).GetAwaiter().GetResult();
        }
        
        public void startLoop(){
            while (true)
            {
                string json = JsonConvert.SerializeObject(new { message = "Heyo :)", sent= DateTimeOffset.UtcNow });
                _mqttClient.PublishAsync("dev.to/topic/json", json);

                Task.Delay(4000).GetAwaiter().GetResult();
            }

        }
         public void sendMessage(String topic, String json){
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
            
            // create the object and send the first message

            IoTEntity pe = new IoTEntity("sensor","uuid", "assembly", "department", null);
            pe.addProperty(new Property("type",111,"temperature",0));
            // serialize
            var jsonnew = JsonConvert.SerializeObject(pe);

            Console.WriteLine(jsonnew);
            // deserialize, this is required on the subscriber side
            IoTEntity ped = JsonConvert.DeserializeObject<IoTEntity>(jsonnew);

            Console.WriteLine(ped.name);
         
            MQTTConnnection mqtt = new MQTTConnnection();
            
            measure(mqtt, pe);
            

        } 
        static void measure(MQTTConnnection mqtt, IoTEntity entity){
            
            while (true)
            {
                // do measurement 
                
                string json = JsonConvert.SerializeObject(new { message = "Heyo :)", sent= DateTimeOffset.UtcNow });

                mqtt.sendMessage("dev.to/topic/json", json);

                Task.Delay(4000).GetAwaiter().GetResult();
            }

        }
}
}
