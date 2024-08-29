import { Component, OnInit } from '@angular/core';
import { IonHeader, IonToolbar, IonSelectOption, IonDatetimeButton, IonModal, IonSelect, IonButton, IonTitle, IonContent, IonItem, IonLabel, IonDatetime } from '@ionic/angular/standalone';
import { SensorChartComponent } from '../components/sensor-chart/sensor-chart.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  standalone: true,
  imports: [CommonModule,FormsModule, IonModal, IonDatetimeButton, IonSelectOption, IonSelect, IonButton, IonHeader, IonToolbar, IonTitle, IonContent, IonItem, IonLabel, IonDatetime, SensorChartComponent ],
})
export class HomePage {
  startDate = undefined;
  endDate = undefined;
  pageSize = 10;
  pageNumber = 0;
  constructor() {}

  previousPage() {
    if (this.pageNumber > 0) {
      this.pageNumber--;
    }
  }

  nextPage() {
    this.pageNumber++;
  }
}
