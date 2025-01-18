// src/router.ts
import { createRouter, createWebHistory } from 'vue-router';
import SensorChart from './components/SensorChart.vue';

const routes = [
  { path: '/', component: SensorChart }
];

const router = createRouter({
  history: createWebHistory((process.env.NODE_ENV === 'production') ? '/vuecharts/' : undefined),
  routes
});

export default router;
