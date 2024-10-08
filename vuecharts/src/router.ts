// src/router.ts
import { createRouter, createWebHistory } from 'vue-router';
import SensorForm from './components/SensorForm.vue'; 
import SensorChart from './components/SensorChart.vue';

const routes = [
  { path: '/', component: SensorChart },
  { path: '/sensor-form', component: SensorForm }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
