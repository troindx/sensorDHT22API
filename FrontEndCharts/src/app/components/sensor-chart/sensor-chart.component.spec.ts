import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { SensorChartComponent } from './sensor-chart.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { SensorDataService } from 'src/app/services/sensor-data.service';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';

jasmine.DEFAULT_TIMEOUT_INTERVAL = 10000; 
describe('SensorChartComponent', () => {
  let component: SensorChartComponent;
  let fixture: ComponentFixture<SensorChartComponent>;
  
  beforeEach(waitForAsync(async () => {
    await TestBed.configureTestingModule({
      imports: [  NgxChartsModule, BrowserAnimationsModule, NoopAnimationsModule, CommonModule, BrowserModule ],
      providers : [provideHttpClient(), provideHttpClientTesting(), SensorDataService]
    }).compileComponents();

    fixture = TestBed.createComponent(SensorChartComponent);
    component = fixture.componentInstance;

    // Spy on the loadSensorData method
    spyOn(component, 'loadSensorData').and.callFake(() => {
      console.log('loadSensorData is mocked');
      return Promise.resolve();
    });

    //fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
