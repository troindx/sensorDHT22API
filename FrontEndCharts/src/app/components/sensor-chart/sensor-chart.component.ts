import { Component, OnInit, Input, OnChanges, SimpleChanges, HostListener } from '@angular/core';
import { SensorDataService, SensorReading } from '../../services/sensor-data.service';
import { Color, NgxChartsModule, ScaleType } from '@swimlane/ngx-charts';
import { CommonModule } from '@angular/common';
import { Toast } from '@capacitor/toast';
import { IonSpinner, IonContent} from '@ionic/angular/standalone';
@Component({
  selector: 'app-sensor-chart',
  templateUrl: './sensor-chart.component.html',
  styleUrls: ['./sensor-chart.component.scss'],
  standalone : true,
  imports: [ NgxChartsModule, CommonModule, IonSpinner, IonContent],
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
  isLoading: boolean = true;
  isFirstLoad: boolean = true;
  
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
    this.loadSensorData(true);
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

  loadSensorData(showLoading: boolean = false) {
    if (showLoading) {
      this.isLoading = true;
    }

    this.sensorDataService
      .getSensorData(this.pageSize, this.pageNumber, this.startDate, this.endDate)
      .subscribe({
        next: (data) => {
          this.sensorData = data;
          this.prepareChartData();
          if (showLoading) {
            this.isLoading = false;
            this.showToastMessage('Data has been loaded');
          }
          this.isFirstLoad = false; // Set to false after the first load
        },
        error: () => {
          if (showLoading) {
            this.isLoading = false;
          }
          this.showToastMessage('Error loading data', true);
        }
      });
  }
  
  
  isValidDate(date: any): boolean {
    // Check if the input is a Date object and is valid
    return date instanceof Date && !isNaN(date.getTime());
  }
  

  prepareChartData() {
    const temperatureSeries = {
      name: 'Temperature',
      series: this.sensorData.map((reading) => ({
        name: this.formatDateWithoutYear(new Date(reading.time)),
        value: reading.temperature,
      })),
    };
  
    const humiditySeries = {
      name: 'Humidity',
      series: this.sensorData.map((reading) => ({
        name: this.formatDateWithoutYear(new Date(reading.time)),
        value: reading.humidity,
      })),
    };
  
    this.chartData = [temperatureSeries, humiditySeries];
  }
  
  formatDateWithoutYear(date: Date): string {
    // Format the date to include only month, day, and time (e.g., "MM/DD HH:mm")
    const options: Intl.DateTimeFormatOptions = { month: '2-digit', day: '2-digit' };
    const datePart = date.toLocaleDateString(undefined, options);
    const timePart = date.toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' });
    return `${datePart} ${timePart}`;
  }
  

  async showToastMessage(message: string, isError: boolean = false) {
    await Toast.show({
      text: message,
      duration: 'short',
      position: 'bottom',
    
    });
  }

}
