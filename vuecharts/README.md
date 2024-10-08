# vuecharts
This project is a vue frontend that takes the data from the apollo server, that feeds itself from the C# endpoint created in the other RPISensor folder.

## Project setup
Make sure you have installed all of the dependencies. you also need graphql-codegen cli tool. it comes bundled. 
Once you execute these commands it should create a folder named gql. 
> Note: before you execute the second command for the first time, you MUST have apollo server (in the ApolloServer folder) up and running. The vue frontend 
uses the feed by apollo server to generate the typescript types.
```
npm install
npx graphql-codegen

```
If you have trouble generating the graphql-codegen. check codegen.ts and either swap the URL for the correct URL used for apollo, or change the URL in the schema option
and place the path to the graphql schema file that you can find in  ../ApolloServer/src/schema.graphql

### Compiles and hot-reloads for development
```
npm run serve
```
you can also run vue serve if you have globally installed vue cli.

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).
