# Apollo Server for RPI Sensor Readings
This server uses apollo to allow for GraphQL to be used in this project.
on first run, in this directory, the following commands:
```
npm i
npm run generate-once
```
Will install all dependencies and generate the types.ts file from the ./src/schema.graphql file
you can run npm run generate so that the tool watches for changes on src/schema.graphql.

## Development mode
You can use this command to check and watch for any changes in the graphql schema or any of the ts files.
```
npm run dev
```
If you change the graphql schema, this will rebuild the files.