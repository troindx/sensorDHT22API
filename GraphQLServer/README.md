# Apollo Server for RPI Sensor Readings
This server uses apollo to allow for GraphQL to be used in this project.
Before starting, you need to copy the .env.dist file in this folder to .env so local env variables are loaded. Then,
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

## Building for prod & CI/CD
When building for production, you must copy the graphql schema into the ./dist folder. Remember this step for your CI/CD or 
the project will not be able to work. If you change the dist folder, then copy the graphql schema in ./src/schema.graphql into whatever 
that folder is. Furthermore, you can also install pm2 manager globally to start the apollo server.
```
npm i pm2 -g
```

### Port swapping in testing.
During test phase, since tests have been designed e2e, the port number is changed in order to launch a server instance for testing in a different port to avoid the error of PORT_ALREADY_IN_USE