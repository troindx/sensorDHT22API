
import type { CodegenConfig } from '@graphql-codegen/cli';

const config: CodegenConfig = {
  overwrite: true,
  schema: 'https://hamrodev.com:4000',
  documents: 'src/**/*.vue',
  generates: {
    'src/gql/': {
      preset: 'client',
      plugins: []
    }
  }
};

export default config;
