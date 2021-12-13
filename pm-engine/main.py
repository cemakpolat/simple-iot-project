import paho.mqtt.client as mqtt
import socket

broker  = "broker"
port = 1883
topic = "/sensors/"
client_id = "pm-engine-1"

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        print("Connected to MQTT Broker!")
       
    else:
        print("Failed to connect, return code %d\n", rc)
    

def on_message(client, userdata, msg):
    print(f"Received `{msg.payload.decode()}` from `{msg.topic}` topic")


client = mqtt.Client(client_id)
client.connect(broker,port,60)

client.on_connect = on_connect
client.on_message = on_message


client.subscribe(topic)
client.loop_forever()

