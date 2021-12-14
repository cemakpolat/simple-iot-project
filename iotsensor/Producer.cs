using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Serilog;
using System.Configuration;
using iotsensor.Services;
using iotsensor.Models;

namespace iotsensor
{   
    class Simulator{
        
        int waitingDuration = 5000; // ms
        MQTTConnnection mqtt = null;
        Util util;
        bool _isRunning = true;
        public Simulator(MQTTConnnection mqtt, int duration){
            this.waitingDuration = duration;
            this.mqtt = mqtt;
            this.util = new Util();
        }
        
        public void run(){
            Log.Information("Device simulation is started...");
            int modeChanger = 30000;
            bool brokenMode = false;
            while(this._isRunning){
                
                measure();
                
                if(modeChanger == 0){
                    if(brokenMode){
                        brokenMode = true;
                    }else{
                        brokenMode = false;
                    }
                    modeChanger = 30000;                    
                }
                modeChanger -=5000;
            }
        }
        
        private void setBroken(){
            this.waitingDuration = waitingDuration * 10; // 30 sec no data
        }
        private void resetDuration(){
            this.waitingDuration = 5000;
        }
        private String createMeasurementObject(){
            Random rnd = new Random();
            IoTEntity pe = new IoTEntity("sensor","uuid", "assembly", "department", null);
            pe.addProperty(new Property("double",  util.getCurrentTime(),"temperature",rnd.Next(5,30)));
            string serialized_entity = JsonConvert.SerializeObject(pe);
            return serialized_entity;
        }
        void measure(){
            // create the object and send the first message
            this.mqtt.sendMessage("/sensors/", createMeasurementObject());
            Task.Delay(this.waitingDuration).GetAwaiter().GetResult();
        }
    }
      
    class Producer
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
            
            int measurement_duration;
            
            bool parsed = int.TryParse(ConfigurationManager.AppSettings.Get("mes_duration"), out measurement_duration);
            if(!parsed){
                measurement_duration = 4000; // default measurement duration
            }
            
            Simulator simulator = new Simulator(new MQTTConnnection(client, url,int.Parse(port)), measurement_duration);
            simulator.run();
            

        } 
        
    }
}
