using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;


namespace DataStore.Domain
{
    public class Value {

        public long timestamp { get; set; }
        public String value { get; set; }

        public Value(long ts, String val){
            this.timestamp = ts;
            this.value = val;
        }
        
    }
    public class Property
    {
        
        public String objectType { get; set; }
        public String key { get; set; }
        public List<Value> values {get;set;}
        public Property(){}
        public Property(String otype, String key, List<Value> vals){
            this.objectType = otype;
            this.key = key;
            this.values = vals;
        }
    
    }
}