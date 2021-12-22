import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BadgeModule, CardModule, GridModule } from '@coreui/angular';
import { ChartjsModule } from '@coreui/angular-chartjs';

import { LogsComponent } from './logs.component';
import { LogsRoutingModule } from './logs-routing.module';

import { TableModule, UtilitiesModule } from '@coreui/angular';

@NgModule({
  declarations: [LogsComponent],
  imports: [
    CommonModule,
    LogsRoutingModule,
    ChartjsModule,
    CardModule,
    GridModule,
    BadgeModule,
    TableModule,
    UtilitiesModule
  ]
})
export class LogsModule {
}
