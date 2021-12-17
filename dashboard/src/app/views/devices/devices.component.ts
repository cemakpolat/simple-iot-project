import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs';
import { EventMqttService } from '../../services/event.mqtt.service';
import { IMqttMessage } from "ngx-mqtt";

@Component({
  templateUrl: 'devices.component.html',
  styleUrls: ['devices.component.scss'],
  providers:[EventMqttService]
})
export class DevicesComponent implements OnInit {

  public title:string ="Temperature Sensor";
  private labels: any[] = [];
  public datalist: any[] = [];
  private deviceId: string;
  chartLineData:any;

  chartLineOptions = {
    maintainAspectRatio: false,
    animation: {
      duration: 0
    }
  };

  private subscription: Subscription;
  constructor(private readonly eventMqtt: EventMqttService) {}
  ngOnInit(): void {
    this.deviceId = "";
    this.subscribeToTopic();
    this.updateLineChart(this.labels, this.datalist);
  }

  ngOnDestroy(): void {
    if (this.subscription) {
        this.subscription.unsubscribe();
    }
}
  updateChartData(tuple:any) {
      if (this.datalist.length > 50){
        this.labels.shift();
        this.datalist.shift();
      }
      let ts = new Date(tuple.x).toISOString().substr(11, 8)
      this.labels.push(ts);
      this.datalist.push(tuple.y)
      this.updateLineChart(this.labels, this.datalist);

  }
  private subscribeToTopic() {
    this.subscription = this.eventMqtt.topic(this.deviceId)
        .subscribe((data: IMqttMessage) => {
          // console.log(data.payload.toString());
          if ( data.payload.toString() != null){
            const entity = JSON.parse(data.payload.toString());
            const properties = entity.properties;
            const item = {"x":properties[0].timestamp, "y":parseFloat(properties[0].value)};
            this.updateChartData(item);
          }
        });
  }
  updateLineChart(labels:any, datalist:any){

    this.chartLineData = {
      labels: labels,
      datasets: [
        {
          label: 'Celcius',
          backgroundColor: '#4dbd74',
          borderColor: '#4dbd74',
          pointBackgroundColor: '#4dbd74',
          pointBorderColor: '#fff',
          data: datalist
        }
      ]
    };
  }



}
