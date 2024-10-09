import { ApolloClient, InMemoryCache, createHttpLink } from '@apollo/client/core';
import { provideApolloClient } from '@vue/apollo-composable';

const customFetch = (uri: RequestInfo | URL, options: RequestInit | undefined) => {
  console.log('Request URI:', uri);
  console.log('Request options:', options);
  return fetch(uri, options);
};
const httpLink = createHttpLink({
  uri: 'https://hamrodev.com/graphql',
  fetch: customFetch
});

const apolloClient = new ApolloClient({
  link: httpLink,
  cache: new InMemoryCache()
});

export function setupApolloClient() {
  provideApolloClient(apolloClient);
}
