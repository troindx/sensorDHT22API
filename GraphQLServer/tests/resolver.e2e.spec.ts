import { ApolloServer } from '@apollo/server';
import { DataSourceContext } from '../src/context';
import { startApolloServer } from '../src/apollo';
import request from 'supertest';
import { SensorReading } from '../src/types';

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
  });

});
