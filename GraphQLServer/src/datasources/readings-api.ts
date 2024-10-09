import { RESTDataSource } from "@apollo/datasource-rest";

import { CreateSensorReadingInput, SensorReading } from "../types";
import dotenv from 'dotenv'
dotenv.config();

export class ReadingsAPI extends RESTDataSource {
  
  baseURL = process.env.API_URL;

  getSensorReadings(pageNumber:number, pageSize:number, dateFrom:string, dateTo:string) {
    if (pageNumber < 0) pageNumber = 0;
    return this.get<SensorReading[]>("sensorreadings", {
      params: {
        pageNumber:pageNumber? pageNumber.toString() : undefined,
        pageSize:pageSize? pageSize.toString() : undefined,
        startDate: dateFrom,
        endDate : dateTo
      }
    });
  }


  createSensorReading(reading: CreateSensorReadingInput): Promise<SensorReading> {
    console.log("Creating sensor reading: ", reading);
    return this.post("sensorreadings", {
      body: {
        Temperature: reading.temperature, 
        Humidity: reading.humidity,
        Time: reading.time
      }
    });
  }

  deleteSensorReading(id: string): Promise<unknown> {
    console.log("Deleting sensor reading with id: ", id);
    return this.delete(`sensorreadings/${id}`);
  }
}