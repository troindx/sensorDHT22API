import { Component, OnInit, Input, OnChanges, SimpleChanges, HostListener } from '@angular/core';
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
export class SensorChartComponent implements OnInit, OnChanges {
  @Input() startDate?: Date;
  @Input() endDate?: Date;
  @Input() pageSize: number = 10;
  @Input() pageNumber: number = 0;

  sensorData: SensorReading[] = [];
  chartData: any[] = [];

  view: [number, number] = [700, 400];
  showXAxis = true;
  showYAxis = true;
  gradient = false;
  showLegend = true;
  showXAxisLabel = true;
  xAxisLabel = 'Date and Time';
  showYAxisLabel = true;
  yAxisLabel = 'Temperature (ºC),  Humidity (%)';
  fillColor: string = '#000000';
  colorSchemeLight: Color = {
    domain: ['#5AA454', '#A10A28', '#C7B42C', '#AAAAAA'],
    group: ScaleType.Ordinal,
    selectable: true,
    name: 'Temperature and Humidity',
  };
  
  colorSchemeDark: Color = {
    domain: ['#1E88E5', '#D32F2F', '#FFC107', '#757575'],
    group: ScaleType.Ordinal,
    selectable: true,
    name: 'Temperature and Humidity',
  };

  colorScheme: Color ;
  constructor(private sensorDataService: SensorDataService) {
    this.colorScheme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
      ? this.colorSchemeDark
      : this.colorSchemeLight;
  }

  ngOnInit() {
    this.updateChartSize();
    this.loadSensorData();
  }


  @HostListener('window:change', ['$event'])
  onThemeChange() {
    this.updateColorScheme();  // Update color scheme when the theme changes
  }

  updateColorScheme() {
    const isDarkMode = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
    this.colorScheme = isDarkMode ? this.colorSchemeDark : this.colorSchemeLight;
    this.fillColor = isDarkMode ? '#ffffff' : '#000000';
  }

  updateChartSize() {
    const width = window.innerWidth * 0.9; // Adjust width as needed (e.g., 90% of window width)
    const height =width * (5/16);  // You can adjust this value or make it dynamic as well
    this.view = [width, height];
  }


  @HostListener('window:resize', ['$event'])
  onResize(event: Event) {
    this.updateChartSize();  // Update view size on window resize
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.loadSensorData();
  }

  loadSensorData() {
    const formattedStartDate = this.isValidDate(this.startDate) ? new Date(this.startDate!) : undefined;
    const formattedEndDate = this.isValidDate(this.endDate) ? new Date(this.endDate!) : undefined;
    this.sensorDataService
      .getSensorData(this.pageSize, this.pageNumber,formattedStartDate, formattedEndDate)
      .subscribe((data) => {
        this.sensorData = data;
        this.prepareChartData();
      });
  }

  isValidDate(date: any): boolean {
    return date instanceof Date && !isNaN(date.getTime());
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
