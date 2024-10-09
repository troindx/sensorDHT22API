import { ApolloServer } from '@apollo/server';
import { DataSourceContext } from '../src/context';
import { startApolloServer } from '../src/apollo';
import request from 'supertest';
import { CreateSensorReadingResponse, DeleteSensorReadingResponse, SensorReading } from '../src/types';

describe('E2E Apollo Server Tests', () => {
  let server: ApolloServer<DataSourceContext>;
  let url: string;
  let createdId : number;
  beforeAll( async () => {
    ( { server , url }  =  await startApolloServer())
  });

  afterAll(async () => {
    await server?.stop();
  });

  it('should create a sensor reading via the actual API', async () => {
    const query = {
      query: `
        mutation CreateSensorReading($input: CreateSensorReadingInput!) {
          createSensorReading(input: $input) {
            code
            message
            sensorReading {
              humidity
              id
              time
              temperature
            }
            success
          }
        }
      `,
      variables: {
        input: { temperature: 25, humidity: 50, time: '2024-10-08T14:02:47' },
      }
    }
    const response = await request(url).post('/').send(query);
    const body = response.body.data.createSensorReading as CreateSensorReadingResponse;
    expect(response.status).toEqual(200);
    expect(body.success).toBe(true);
    expect(body.code).toBe(200);
    expect(body.sensorReading).toBeDefined();
    expect(body.sensorReading?.temperature).toBe(25);
    expect(body.sensorReading?.humidity).toBe(50);
    expect(body.sensorReading?.id).toBeDefined();
    createdId = parseInt(body.sensorReading!.id);
  });

  it('should retrieve all of the sensor readings via API', async () => {
    const query = {
      query: `
        query SensorReadings($pageSize: Int, $pageNumber: Int) {
          sensorReadings(pageSize: $pageSize, pageNumber: $pageNumber) {
            humidity
            id
            temperature
            time
          }
        }
      `,
      variables: {
        pageNumber: 0,
        pageSize: 5
      }
    }
    const response = await request(url).post('/').send(query);
    const body = response.body.data.sensorReadings as SensorReading[];
    expect(response.status).toBe(200);
    expect(body.length).toBe(5);
    expect(body.filter(reading => parseInt(reading.id) === createdId).length).toBe(1);
  });

  it('should delete the reading via exposed Apollo API', async () => {
    const query = {
      query: `
        mutation deleteSensorReading($id: ID!) {
          deleteSensorReading(id: $id) {
            id
            code
            message
          }
      }
      `,
      variables: {
        id: createdId.toString()
      }
    }
    const response = await request(url).post('/').send(query);
    const body = response.body.data.deleteSensorReading as DeleteSensorReadingResponse;
    expect(response.status).toEqual(200);
    expect(body.code).toBe(200);
    expect(body.id).toBeDefined();
    expect(body.id).toBe(createdId.toString());
  });

});
