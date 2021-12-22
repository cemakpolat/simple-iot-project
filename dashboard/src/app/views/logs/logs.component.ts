
import {Component, OnInit} from '@angular/core';
import { EventMqttService } from '../../services/event.mqtt.service';
import { IMqttMessage } from "ngx-mqtt";
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-charts',
  templateUrl: './logs.component.html',
  styleUrls: ['./logs.component.scss'],
  providers:[EventMqttService]
})
export class LogsComponent  implements OnInit {
  public fieldArray: Array<any> = [];
  private topic = "logs"

  private subscription: Subscription;
  constructor(private readonly eventMqtt: EventMqttService){}
  ngOnInit(): void {
    this.subscribeToTopic();
  }

  addFieldValue(item:any) {
     if (this.fieldArray.length > 50){
        this.fieldArray.shift();
      }
      var attr = {
        "component":item.component,
        "timesamp":item.timestamp,
        "message":item.message
      };
      this.fieldArray.push(attr)
  }
private subscribeToTopic() {
    this.subscription = this.eventMqtt.topic(this.topic)
        .subscribe((data: IMqttMessage) => {

          const item = JSON.parse(data.payload.toString());
          // console.log(item);
          this.addFieldValue(item);
        });
  }

}
