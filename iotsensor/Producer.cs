using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using System.Configuration;
using iotsensor.Services;
using iotsensor.Models;

namespace iotsensor
{   
    class Simulator{
        
        int waitingDuration = 2000; // ms
        MQTTConnnection mqtt = null;
        Util util;
        bool _isRunning = true;
        int simulationWaitingDuration = 10000;
        public Simulator(MQTTConnnection mqtt, int waitingDuration, int simulationWaitingDuration){
            this.waitingDuration = waitingDuration;
            this.mqtt = mqtt;
            this.util = new Util();
            this.simulationWaitingDuration = simulationWaitingDuration;
        }
        
        public void run(){
            
            Log.Logger.Information("Device simulation is started..." );
            int counter = this.simulationWaitingDuration/this.waitingDuration;
            while(this._isRunning){
                if(counter == 0){
                    int dur = this.simulationWaitingDuration - this.waitingDuration;
                    Log.Logger.Information("Data Streaming stops for "+ dur/1000 + " seconds");
                    Thread.Sleep(dur);
                    counter = this.simulationWaitingDuration/this.waitingDuration;
                    Log.Logger.Information("Data Streaming starts again...");                    
                }
                measure();
                Thread.Sleep(this.waitingDuration);
                counter = counter - 1;
            }
        }
    
        private String createMeasurementObject(){
            Random rnd = new Random();
            IoTEntity pe = new IoTEntity("sensor","uuid", "assembly", "department", null);
            pe.addProperty(new Property("double",  util.getCurrentTime(),"temperature",rnd.Next(10,30)));
            string serialized_entity = JsonConvert.SerializeObject(pe);
            return serialized_entity;
        }
        void measure(){
            // create the object and send the first message
            this.mqtt.sendMessage("/sensors/", createMeasurementObject());
            Task.Delay(10).GetAwaiter().GetResult();
            
        }
    }
      
    class Producer
    {        
         static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            
            
            String url = ConfigurationManager.AppSettings.Get("brokerUrl");
            String port = ConfigurationManager.AppSettings.Get("brokerPort");
            String client = ConfigurationManager.AppSettings.Get("mqttClientId");
            
            int measurement_duration;
            int sim_mode_changing_duration;
            
            bool parsed = int.TryParse(ConfigurationManager.AppSettings.Get("waiting_duration"), out measurement_duration);
            bool parsed_sim = int.TryParse(ConfigurationManager.AppSettings.Get("sim_mode_changing_duration"), out sim_mode_changing_duration);
            
            if(!parsed){
                measurement_duration = 500; // default measurement duration
            }
            if(!parsed_sim){
                sim_mode_changing_duration = 10000; // default measurement duration
            }
            
            Simulator simulator = new Simulator(new MQTTConnnection(client, url,int.Parse(port)), measurement_duration,sim_mode_changing_duration);
            simulator.run();
            

        } 
        
    }
}
