import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Paho } from 'ng2-mqtt/mqttws31';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private mqttClient: Paho.MQTT.Client;
  topicname: any;
  msg: any;
  isConnected: boolean = false;
  @ViewChild('msglog', {static: true}) msglog: ElementRef;

  constructor() {}
  
  ngOnInit(): void {
    // Broker connection on startup
    this.connectToMQTTBroker();
  }

  public connectToMQTTBroker() {
    let clientId = 'mqttjs_' + Math.random.toString().substr(2, 8);
    this.logMsg('trying to connect: '+ clientId);
    this.mqttClient = new Paho.MQTT.Client('127.0.0.1/ws', clientId);
    
    // Connection lost
    this.mqttClient.onConnectionLost = (responseObject: Object) => {
      this.logMsg('Connection with clientId ' + clientId + 'is lost.');
    }

    // Message received
    this.mqttClient.onMessageArrived = (message: Paho.MQTT.Message) => {
      this.logMsg('Message arrived: ' + message.payloadString + '<br>Topic: ' + message.destinationName);
    }

    this.mqttClient.connect({ onSuccess: this.onConnected.bind(this) });
  }

  subscribeNewTopic(): void {
    if (this.isConnected) {
      this.mqttClient.subscribe(this.topicname, {});
      this.logMsg('Subscribed to topic: ' + this.topicname);
    } else {
      this.logMsg('Broker is not connected yet hence unable to subscribe to the topic!');
    }
  }

  sendmsg(): void {
    if (this.isConnected) {
      if (typeof (this.topicname) === 'undefined' || this.topicname === null || this.topicname === '') {
        this.logMsg('Please subcribe to atleast one topic!');
        return;
      }
      let message = new Paho.MQTT.Message(this.msg);
      message.destinationName = this.topicname;
      this.mqttClient.send(message);
      this.logMsg('Message' + message.payloadString + ' published to the topic ' + this.topicname);
    } else {
      this.logMsg('Broker is not connected yet hence unable to publish to the topic!');
    }
  }

  onConnected(): void {
    this.logMsg('Connected to broker!');
    this.isConnected = true;
  }

  logMsg(message): void {
    this.msglog.nativeElement.innerHTML += '<br><hr>' + message;
  }

  clear(): void {
    this.msglog.nativeElement.innerHTML = '';
  }
}
