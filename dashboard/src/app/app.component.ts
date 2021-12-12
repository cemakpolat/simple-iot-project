import { Component, OnInit, ViewChild, ElementRef, OnDestroy} from '@angular/core';
import {  Subscription } from 'rxjs';
import { IMqttMessage, MqttService } from 'ngx-mqtt';
import {Point} from "chart.js";

import {LineChartComponent} from './line-chart/line-chart.component'


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  
  title = 'IoT Dashboard';  
  private subscription: Subscription;
  topicname: any;
  msg: any;
  isConnected: boolean = false;
  @ViewChild('msglog', { static: true }) msglog: ElementRef;
  @ViewChild(LineChartComponent,{ static: true }) lineCanvas: LineChartComponent;

  constructor(private _mqttService: MqttService) {}

  ngOnInit(): void {
    this.subscribeNewTopic("/sensors/");
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }


  subscribeNewTopic(topic): void {
    this.subscription = this._mqttService.observe(topic).subscribe((message: IMqttMessage) => {
      this.msg = message;

      // this.logMsg('Message: ' + message.payload.toString() + '<br> for topic: ' + message.topic);
      if ( message.payload.toString() != null){
        const entity = JSON.parse(message.payload.toString());
        const properties = entity.properties;
        this.lineCanvas.updateChartData({"x":properties[0].timestamp, "y":parseFloat(properties[0].value)});
      }

    });
    // this.logMsg('subscribed to topic: ' + this.topicname)
  }
  
  sendmsg(): void {
    // use unsafe publish for non-ssl websockets
    this._mqttService.unsafePublish(this.topicname, this.msg, { qos: 1, retain: true })
    this.msg = ''
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
