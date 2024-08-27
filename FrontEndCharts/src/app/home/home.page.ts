import { Component } from '@angular/core';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular/standalone';
import { SensorChartComponent } from '../components/sensor-chart/sensor-chart.component';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [IonHeader, IonToolbar, IonTitle, IonContent, SensorChartComponent ],
})
export class HomePage {
  startDate = undefined;
  endDate = undefined;
  constructor() {}
}
