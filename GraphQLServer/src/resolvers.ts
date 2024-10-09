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
  },
  Mutation: {
    createSensorReading: async (_, { input }, { dataSources }) => {
      try {
        const response = await dataSources.readingsAPI.createSensorReading(input);
        return {
          code: 200,
          success: true,
          message: "SensorReading successfully created!",
          sensorReading: response
        };
      } catch (err) {
        return {
          code: 500,
          success: false,
          message: `Something went wrong: ${err.extensions.response.body}`,
          sensorReading: null
        };
      }
    },
    deleteSensorReading: async (_, { id }, { dataSources }) => {
      interface errorResponse {
        type:string,
        title: string,
        status: number,
        traceId: string
      }

      try {
        await dataSources.readingsAPI.deleteSensorReading(id);
        return {
          code: 200,
          success: true,
          message: "SensorReading successfully deleted!",
          id: id
        };
      } catch (err) {
        console.log(err)
        const resp:errorResponse = JSON.parse(err.extensions.response.body);
        return {
          code: resp.status,
          success: false,
          message: `Something went wrong: ${resp.title}, ${resp.type}, ${resp.traceId} `,
          id: null
        };
      }
    }
  },
}