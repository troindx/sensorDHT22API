import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment.prod';

export interface SensorReading {
  id: number;
  time: string;
  temperature: number;
  humidity: number;
}

@Injectable({
  providedIn: 'root'
})
export class SensorDataService {

  constructor(private http: HttpClient) { }

  getSensorData(pageSize: number = 10, pageNumber: number = 0, startDate?: Date, endDate?: Date): Observable<SensorReading[]> {
    let params = new HttpParams()
      .set('pageSize', pageSize.toString())
      .set('pageNumber', pageNumber.toString());

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }

    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<SensorReading[]>(environment.apiURL, { params });
  }
}
