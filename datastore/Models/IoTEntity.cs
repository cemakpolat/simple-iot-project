using System;
using System.Collections.Generic;
namespace DataStore.Models
{
  class IoTEntity {

        
        public String objectType { get; set; }
        public String uuid { get; set; }
        public String groupId { get; set; }
        public String name { get; set; }
        public List<IoTProperty> properties = new List<IoTProperty>();
        public IoTEntity(){}

        public IoTEntity(String oType, String uuid,  String name, String groupId, List<IoTProperty> properties) {
            this.objectType = oType;
            this.uuid = uuid;
            this.groupId = groupId;
            this.name = name;
            if(properties != null){
                this.properties = properties;
            }
            
        }
        public IoTEntity addProperty(IoTProperty item){
            bool exist = false;
            foreach(IoTProperty element in this.properties){
                if (element.key == item.key){
                    exist = true;
                    break;
                }
            }
            if (exist == false){
                this.properties.Add(item);
            }
            return this;
        }

    }
}