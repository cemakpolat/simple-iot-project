using System.Collections.Generic;
using System;
using common_api;
namespace common_api
{
  public class IoTEntity {

        
        public String objectType { get; set; }
        public String uuid { get; set; }
        public String groupId { get; set; }
        public String name { get; set; }
        public List<Property> properties = new List<Property>();
        public IoTEntity(){}

        public IoTEntity(String oType, String uuid,  String name, String groupId, List<Property> properties) {
            this.objectType = oType;
            this.uuid = uuid;
            this.groupId = groupId;
            this.name = name;
            if(properties != null){
                this.properties = properties;
            }
            
        }
        public IoTEntity addProperty(Property item){
            bool exist = false;
            foreach(Property element in this.properties){
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