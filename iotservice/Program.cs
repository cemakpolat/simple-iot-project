using System;

using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using iotservice.Services;

using System.Configuration;
using System.Collections.Specialized;
namespace iotservice
{    
    class Program
    {

         static async  Task Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            
            String url = ConfigurationManager.AppSettings.Get("brokerUrl");
            String port = ConfigurationManager.AppSettings.Get("brokerPort");
            String client = ConfigurationManager.AppSettings.Get("mqttClientId");
            
            MQTTConnnection mqtt = new MQTTConnnection(client, url,int.Parse(port));
            
            await Task.Run(() => Thread.Sleep(Timeout.Infinite));

        }
    }


   
}
