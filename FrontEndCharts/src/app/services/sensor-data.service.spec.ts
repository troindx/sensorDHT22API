import { TestBed } from '@angular/core/testing';

import { SensorDataService } from './sensor-data.service';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
describe('SensorDataService', () => {
  let service: SensorDataService;

  beforeEach(() => {
    TestBed.configureTestingModule({providers: [
      provideHttpClient(),
      provideHttpClientTesting(),]});
    service = TestBed.inject(SensorDataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
