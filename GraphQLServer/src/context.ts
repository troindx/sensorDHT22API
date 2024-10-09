import { ReadingsAPI } from "./datasources/readings-api";

export type DataSourceContext = {
  dataSources: {
    readingsAPI: ReadingsAPI;
  };
};