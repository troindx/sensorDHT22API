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
      const date = new Date(startDate);
      params = params.set('startDate', this.formatDateToBackend(date));
    }

    if (endDate) {
      const date = new Date(endDate);
      params = params.set('endDate', this.formatDateToBackend(date));
    }

    return this.http.get<SensorReading[]>(environment.apiURL, { params });
  }

  private formatDateToBackend(date: Date): string {
    // Format the date to "yyyy-MM-ddTHH:mm:ss"
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-based
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');
  
    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
  }
}
