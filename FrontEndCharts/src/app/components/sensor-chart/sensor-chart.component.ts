import { Component, OnInit, Input } from '@angular/core';
import { SensorDataService, SensorReading } from '../../services/sensor-data.service';
import { Color, NgxChartsModule, ScaleType } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sensor-chart',
  templateUrl: './sensor-chart.component.html',
  styleUrls: ['./sensor-chart.component.scss'],
  standalone : true,
  imports: [ NgxChartsModule, CommonModule],
})
export class SensorChartComponent implements OnInit {
  @Input() startDate?: Date;
  @Input() endDate?: Date;
  @Input() pageSize: number = 20;
  @Input() pageNumber: number = 0;

  sensorData: SensorReading[] = [];
  chartData: any[] = [];

  view: [number, number] = [700, 400];
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  xAxisLabel = 'Time';
  showYAxisLabel = true;
  yAxisLabel = 'Value';
  colorScheme: Color = {
    domain: ['#5AA454', '#C7B42C', '#A10A28', '#AAAAAA'],
    group: ScaleType.Ordinal,
    selectable: true,
    name: 'Temperature and Humidity',
  };

  constructor(private sensorDataService: SensorDataService) {}

  ngOnInit() {
    this.loadSensorData();
  }

  loadSensorData() {
    this.sensorDataService
      .getSensorData(this.pageSize, this.pageNumber, this.startDate, this.endDate)
      .subscribe((data) => {
        this.sensorData = data;
        this.prepareChartData();
      });
  }

  prepareChartData() {
    const temperatureSeries = {
      name: 'Temperature',
      series: this.sensorData.map((reading) => ({
        name: new Date(reading.time).toLocaleString(),
        value: reading.temperature,
      })),
    };

    const humiditySeries = {
      name: 'Humidity',
      series: this.sensorData.map((reading) => ({
        name: new Date(reading.time).toLocaleString(),
        value: reading.humidity,
      })),
    };

    this.chartData = [temperatureSeries, humiditySeries];
  }
}
