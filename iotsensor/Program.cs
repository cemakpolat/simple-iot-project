using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using iotsensor.Services;
using iotsensor.Models;

namespace iotsensor
{   
      
      
    class Program
    {
        
         static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            
            String url = ConfigurationManager.AppSettings.Get("brokerUrl");
            String port = ConfigurationManager.AppSettings.Get("brokerPort");
            String client = ConfigurationManager.AppSettings.Get("mqttClientId");
            int mesurement_duration = int.Parse(ConfigurationManager.AppSettings.Get("mes_duration"));
            
            MQTTConnnection mqtt = new MQTTConnnection(client, url,int.Parse(port));
                        
            measure(mqtt,mesurement_duration);
            

        } 
        static void measure(MQTTConnnection mqtt, int duration){
            
            while (true)
            {
                // create the object and send the first message
                Random rnd = new Random();
                
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                IoTEntity pe = new IoTEntity("sensor","uuid", "assembly", "department", null);
                pe.addProperty(new Property("double",  milliseconds,"temperature",rnd.Next(5,30)));
                
                string serialized_obj = JsonConvert.SerializeObject(pe);

                mqtt.sendMessage("/sensors/", serialized_obj);

                Task.Delay(duration).GetAwaiter().GetResult();
            }

        }
}
}
