using System;
using System.Collections.Generic;
namespace DataStore.Models
{
     public class IoTProperty  {
        public String objectType { get; set; }

        public long timestamp { get; set; }
        public String value { get; set; }
        public String key { get; set; }
        public IoTProperty(){}
        public IoTProperty(String otype, long tstamp, String key, String value){
            this.objectType = otype;
            this.timestamp = tstamp;
            this.key = key;
            this.value = value;
        }
    
    }
}