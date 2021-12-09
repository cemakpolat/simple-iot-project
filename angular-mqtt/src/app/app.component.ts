import { Component, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Paho } from 'ng2-mqtt/mqttws31';
import { Observable, Subscription } from 'rxjs';
import { IMqttMessage, MqttService } from 'ngx-mqtt';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  // private mqttClient: Paho.MQTT.Client;
  private subscription: Subscription;
  private message: String;
  topicname: any;
  msg: any;
  isConnected: boolean = false;
  @ViewChild('msglog', { static: true }) msglog: ElementRef;

  constructor(private _mqttService: MqttService) { }

  ngOnInit(): void {
    // Broker connection on startup
    // this.connectToMQTTBroker();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }


  subscribeNewTopic(): void {
    console.log('inside subscribe new topic')
    this.subscription = this._mqttService.observe(this.topicname).subscribe((message: IMqttMessage) => {
      this.msg = message;
      console.log('msg: ', message)
      this.logMsg('Message: ' + message.payload.toString() + '<br> for topic: ' + message.topic);
    });
    this.logMsg('subscribed to topic: ' + this.topicname)
  }

  sendmsg(): void {
    // use unsafe publish for non-ssl websockets
    this._mqttService.unsafePublish(this.topicname, this.msg, { qos: 1, retain: true })
    this.msg = ''
  }

  // onConnected(): void {
  //   this.logMsg('Connected to broker!');
  //   this.isConnected = true;
  // }

  logMsg(message): void {
    this.msglog.nativeElement.innerHTML += '<br><hr>' + message;
  }

  clear(): void {
    this.msglog.nativeElement.innerHTML = '';
  }
}
