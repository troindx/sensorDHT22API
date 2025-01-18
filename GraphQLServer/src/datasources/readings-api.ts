import { RESTDataSource } from "@apollo/datasource-rest";

import { SensorReading } from "../types";
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

}