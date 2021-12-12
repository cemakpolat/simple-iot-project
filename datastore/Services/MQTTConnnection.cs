using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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

namespace DataStore.Services
{
 class MQTTConnnection{
        IConfiguration Configuration { get; }
        IManagedMqttClient _mqttClient  = null;
        String mqttClient;
        String brokerIp;
        int brokerPort;
        ManagedMqttClientOptions options = null;
        private  IoTService _iotService;
        private String cstring; 
        private String database; 
        public MQTTConnnection(IConfiguration configuration){
            Configuration = configuration;
            this.setSettings();
            this.setup(mqttClient, brokerIp, brokerPort);
            
        }
        public void setSettings(){
            this.cstring = Configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value;
            this.database = Configuration.GetSection("DatabaseSettings").GetSection("Database").Value;
            this.brokerIp = Configuration.GetSection("MQTTSettings").GetSection("BrokerURL").Value;
            this.brokerPort = int.Parse(Configuration.GetSection("MQTTSettings").GetSection("BrokerPort").Value);
            this.mqttClient = Configuration.GetSection("MQTTSettings").GetSection("MQTTClientID").Value;
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


            _mqttClient.UseApplicationMessageReceivedHandler(e => handleMessage(e.ApplicationMessage));
            _mqttClient.StartAsync(options).GetAwaiter().GetResult();
            this.subscribe("dev.to/topic/json");

           
        }
        private async void handleMessage(MqttApplicationMessage message){
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {message.Topic}");
                
                DataStore.Models.IoTEntity entity = JsonConvert.DeserializeObject<DataStore.Models.IoTEntity>(Encoding.UTF8.GetString(message.Payload));
                // DataStore.Domain.Entity entity = JsonConvert.DeserializeObject<DataStore.Domain.Entity>(Encoding.UTF8.GetString(message.Payload));
                // String json = JsonConvert.SerializeObject(entity);
                // Console.WriteLine(json);
                // Console.WriteLine(entity.properties[0].key);
                //store.insert(Encoding.UTF8.GetString(message.Payload));
                
                
                DataStore.Domain.Entity  et = new DataStore.Domain.Entity();
                et.name = entity.name;
                et.objectType = entity.objectType;
                et.uuid = entity.uuid;
                et.groupId = entity.groupId;
                et.properties = new List<DataStore.Domain.Property>();
                foreach(DataStore.Models.IoTProperty prop in entity.properties){
                    List<DataStore.Domain.Value> list = new List<DataStore.Domain.Value>();
                    list.Add(new DataStore.Domain.Value(prop.timestamp, prop.value));
                    et.properties.Add(new DataStore.Domain.Property(prop.objectType, prop.key,list));
                }
                
                _iotService = new IoTService(this.cstring, this.database);
                await _iotService.Insert(et);
                
        }
        


        public void subscribe(String topic){
            this._mqttClient.SubscribeAsync(
             new MqttTopicFilter
                 {
                    Topic = topic 
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
}