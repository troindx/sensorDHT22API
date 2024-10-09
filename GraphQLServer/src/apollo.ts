import { ApolloServer } from "@apollo/server";
import { startStandaloneServer } from "@apollo/server/standalone";
import { readFileSync } from "fs";
import path from "path";
import { gql } from "graphql-tag";
import { resolvers } from "./resolvers";
import { ReadingsAPI } from "./datasources/readings-api";

// Read the port from command-line arguments or environment variable, default to 4000
const DEFAULT_PORT = 4000;
const args = process.argv.slice(2);
const portArg = args.find(arg => arg.startsWith('--port='));
const PORT = portArg ? parseInt(portArg.split('=')[1], 10) : process.env.PORT || DEFAULT_PORT;

export const typeDefs = gql(
  readFileSync(path.resolve(__dirname, "./schema.graphql"), {
    encoding: "utf-8",
  })
);

export async function startApolloServer() {
  const server = new ApolloServer({ 
    typeDefs, 
    resolvers, 
    introspection: true ,
    csrfPrevention:true, 
    formatError: (err) => {
      console.error(err);
      return err;
    } 
  });
  const { url } = await startStandaloneServer(server, {
    context: async () => {
      const { cache } = server;
      return {
        dataSources: {
          readingsAPI: new ReadingsAPI({ cache }),
        },
      };
    },
    listen: { port: Number(PORT) }, // Set port here using environment variable or default
  });

  console.log(`
    🚀  Server is running on port ${PORT}!
    📭  Query at ${url}
  `);

  return { server, url };
}
