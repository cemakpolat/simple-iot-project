using System.Collections.Generic;
using System;
namespace iotservice.Models
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
}