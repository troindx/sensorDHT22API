import { createApp } from 'vue';
import App from './App.vue';
import { setupApolloClient } from './apollo';
import vuetify from './plugins/vuetify'; 
import router from './router';
setupApolloClient();
createApp(App)
  .use(vuetify)
  .use(router)
  .mount('#app');
