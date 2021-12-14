import paho.mqtt.client as mqtt
import socket
import logging

logging.basicConfig(level=logging.INFO)

broker  = "broker"
port = 1883
topic = "/sensors/"
client_id = "pm-engine-1"

def on_connect(client, userdata, flags, rc):
    if rc == 0:
        logging.info("Connected to MQTT Broker!")
       
    else:
        logging.info("Failed to connect, return code %d\n", rc)
    

def on_message(client, userdata, msg):
    logging.info(f"Received `{msg.payload.decode()}` from `{msg.topic}` topic")


client = mqtt.Client(client_id)
client.connect(broker,port,60)

client.on_connect = on_connect
client.on_message = on_message


client.subscribe(topic)
client.loop_forever()

