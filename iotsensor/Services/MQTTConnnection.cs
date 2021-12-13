using System.Collections.Generic;
using System;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;

namespace iotsensor.Services
{
    class MQTTConnnection{
        IManagedMqttClient _mqttClient  = null;
        String client;
        String brokerUrl;
        int brokerPort = 1883;
        ManagedMqttClientOptions options = null;
        public MQTTConnnection(){}
        public MQTTConnnection(String client, String broker, int port){
            this.client = client;
            this.brokerUrl = broker;
            this.brokerPort = port;
            this.setup();
        }
        public void setup(){
            this.setup(this.client, this.brokerUrl, this.brokerPort);
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
}