import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomePage } from './home.page';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { SensorDataService } from '../services/sensor-data.service';
import { IonHeader, IonToolbar, IonTitle, IonContent } from '@ionic/angular/standalone';
import { SensorChartComponent } from '../components/sensor-chart/sensor-chart.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
describe('HomePage', () => {
  let component: HomePage;
  let fixture: ComponentFixture<HomePage>;

  beforeEach(async () => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(),
                  provideHttpClientTesting(),
      ],
      imports:[IonHeader, IonToolbar, IonTitle, IonContent, SensorChartComponent , BrowserAnimationsModule]});
    let service = TestBed.inject(SensorDataService);
    TestBed.compileComponents();
    fixture = TestBed.createComponent(HomePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
