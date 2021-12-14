using System;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Serilog;
using iotservice.Services;
using System.Configuration;

namespace iotservice
{   
    class Consumer
    {

        private static bool _isDeviceStreamingData = true;
        private static bool _isRunning = true;
        private static long _maxTimeDuration = 30*1000; // 30 sn
        static async  Task Main(string[] args) {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            String url = ConfigurationManager.AppSettings.Get("brokerUrl");
            String port = ConfigurationManager.AppSettings.Get("brokerPort");
            String client = ConfigurationManager.AppSettings.Get("mqttClientId");
            
            MQTTConnnection mqtt = new MQTTConnnection(client, url,int.Parse(port));
            Consumer consumer = new Consumer();
            
            Thread deviceObserver = new Thread(consumer.observeDeviceIsWorking);  
            deviceObserver.Start(mqtt);
                        
            // Thread deviceObserver = new Thread(new ThreadStart(observeDeviceIsWorking));  
            // deviceObserver.Start();
            await Task.Run(() => Thread.Sleep(Timeout.Infinite));

        }
        
        private  void observeDeviceIsWorking(object connection)  
        {  
            MQTTConnnection mqtt = (MQTTConnnection)connection;
            Util util = new Util();
            while (_isRunning)  
            {  
                    long timeDifference = util.getCurrentTime() - mqtt.lastReceivedMessageTimestamp;
                    if (timeDifference >= _maxTimeDuration){
                        Log.Information("Message cannot be received...");
                        mqtt.sendMessage("/sensor/error/","Message cannot be received...");
                    }
            }  
        }  
    }


   
}
