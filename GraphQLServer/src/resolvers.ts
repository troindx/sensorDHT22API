import { Resolvers } from './types'

export const resolvers: Resolvers = {
  Query: {
    sensorReadings: (_, {pageNumber, pageSize, dateFrom, dateTo}, {dataSources}) => {
      return dataSources.readingsAPI.getSensorReadings(pageNumber, pageSize, dateFrom, dateTo);
    },

    currentTime: () => {
      const date = new Date();    
      // Get the date part in the "YYYY-MM-DD" format
      const dateString = date.toISOString().split('T')[0];    
      // Get the time part in "HH:MM:SS" format by removing the milliseconds and the 'Z'
      const timeString = date.toTimeString().split(' ')[0];   
      return `${dateString}T${timeString}`;
    }
  }
}