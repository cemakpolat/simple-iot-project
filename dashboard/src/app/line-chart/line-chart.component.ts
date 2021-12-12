import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import * as Chart from 'chart.js';

@Component({
  selector: 'app-line-chart',
  templateUrl: './line-chart.component.html',
  styleUrls: ['./line-chart.component.scss']
})

export class LineChartComponent implements AfterViewInit {
  @ViewChild('lineCanvas',{static: true}) lineCanvas: ElementRef;
  // private chartRef: ElementRef;
  private chart: Chart;
  private data: any[] = [];
  private labels: any[] = [];

  constructor() {}

  updateChartData(tuple:any) {  
    console.log("tuple",this.data, this.labels);
      if (this.data.length > 200){
        this.data.shift();
        this.labels.shift();
      }

      this.labels.push(tuple.x);
      this.data.push(tuple.y)  

      this.chart.update();
  }
  
  ngAfterViewInit() {

    this.chart = new Chart(this.lineCanvas.nativeElement, {
            type: 'line',
            data: {
              labels: this.labels,//Array.from({length: this.data.length}, (v, k) => k+1),
              datasets: [
                {
                  label: 'Sensor Data',
                  fill: false,
                  lineTension: 1,
                  backgroundColor: 'rgba(75,192,192,0.4)',
                  borderColor: 'rgba(75,192,192,1)',
                  borderCapStyle: 'butt',
                  borderDash: [],
                  borderDashOffset: 0,
                  borderJoinStyle: 'miter',
                  pointBorderColor: 'rgba(75,192,192,1)',
                  pointBackgroundColor: '#fff',
                  pointBorderWidth: 1,
                  pointHoverRadius: 5,
                  pointHoverBackgroundColor: 'rgba(75,192,192,1)',
                  pointHoverBorderColor: 'rgba(220,220,220,1)',
                  pointHoverBorderWidth: 2,
                  pointRadius: 1,
                  pointHitRadius: 10,
                  data: this.data,
                  spanGaps: false,
                }
              ]
            },
            options: {
              scales: {
                 yAxes: [{
                    ticks: {
                       beginAtZero: true
                    }
                 }]
              }
            }
          });
   }

  ngOnDestroy(): void {
  }
}
