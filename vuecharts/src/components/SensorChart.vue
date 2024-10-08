<template>
  <v-container>
    <v-form>
      <v-row>
        <v-col cols="12" md="6">
          <v-text-field
            v-model="pageSize"
            label="Page Size"
            type="number"
            @input="reQuery"
          ></v-text-field>
        </v-col>

        <v-col cols="12" md="6">
          <v-text-field
            v-model="pageNumber"
            label="Page Number"
            type="number"
            @input="reQuery"
          ></v-text-field>
        </v-col>

        <v-col cols="12" md="6">
          <v-date-picker
            title="Date From"
            v-model="dateFrom"
          ></v-date-picker>
        </v-col>

        <v-col cols="12" md="6">
          <v-date-picker
            title="Date To"
            v-model="dateTo"
            @input="reQuery"
          ></v-date-picker>
        </v-col>
      </v-row>
    </v-form>

    <LineChart v-if="chartData" :data="chartData" :options="chartOptions" />
    <br/>
    <v-row>
      <v-col>
        <v-btn  @click="navigateToForm">Add Sensor Reading</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>

import { ref, watch, defineComponent } from 'vue';
import { useQuery } from '@vue/apollo-composable';
import { gql } from '@apollo/client/core';
import { Line as LineChart } from 'vue-chartjs';
import { Chart, LineController, LineElement, PointElement, LinearScale, Title, CategoryScale, Tooltip, Legend } from 'chart.js';
import { formatDateToBackend } from '@/util/dateUtils';
import { useRouter } from 'vue-router';
Chart.register(LineController, LineElement, PointElement, LinearScale, Title, CategoryScale, Tooltip, Legend);

const shortenDateForView = (date) => {
  const month = date.toLocaleString('default', { month: 'short' }); // Get month abbreviation
  const day = date.getDate();
  const hours = date.getHours(); 
  const minutes = date.getMinutes(); 

  const formattedMinutes = minutes < 10 ? `0${minutes}` : minutes;

  return `${day}/${month} - ${hours}:${formattedMinutes}`;
};

export default defineComponent({
  components: {
    LineChart
  },

  beforeRouteEnter(to, from, next) {
    console.log('Returning to original page');
    next(vm => {
      vm.reQuery();
    }); 
  },

  setup() {
    const router = useRouter();
    const pageSize = ref(20);
    const pageNumber = ref(0);
    const dateFrom = ref(null);
    const dateTo = ref(null);
    const chartData = ref({
      labels: [],
      datasets: []
    });

    const chartOptions = ref({
      responsive: true,
      scales: {
        y: {
          min: 0,
          max: 100,
          title: {
            display: true,
            text: 'Temp and Humidity (C and %)'
          }
        },
        x: {
          title: {
            display: true,
            text: 'Time'
          }
        }
      },
      plugins: {
        title: {
          display: true,
          text: 'Temperature and Humidity', // Title of the chart
          font: {
            size: 28 // You can adjust the font size or other styles
          }
        }
      }
    });

    const SENSOR_READINGS_QUERY = gql`
      query SensorReadings($pageSize: Int, $pageNumber: Int, $dateFrom: Date, $dateTo: Date) {
        sensorReadings(pageSize: $pageSize, pageNumber: $pageNumber, dateFrom: $dateFrom, dateTo: $dateTo) {
          id
          temperature
          humidity
          time
        }
      }
    `;

    const { result, refetch } = useQuery(SENSOR_READINGS_QUERY, {
      pageSize: pageSize.value,
      pageNumber: pageNumber.value,
      dateFrom: undefined,
      dateTo: undefined
    });

    const reQuery = () => {
      console.log('Triggering requery with: ', pageNumber.value, pageSize.value, dateFrom.value, dateTo.value);
      refetch({
        pageSize: parseInt(pageSize.value),
        pageNumber: parseInt(pageNumber.value),
        dateFrom: dateFrom.value ? formatDateToBackend(new Date(dateFrom.value)) : undefined,
        dateTo: dateTo.value ? formatDateToBackend(new Date(dateTo.value)) : undefined
      });
    };

    watch(dateFrom, () => {
      reQuery();
    });
    watch(dateTo, () => {
      reQuery();
    });

    watch(result, (newResult) => {
      if (newResult && newResult.sensorReadings) {
        console.log('[Updated Apollo query]', newResult);
        chartData.value = {
          labels: newResult.sensorReadings.map(reading => shortenDateForView(new Date(reading.time))),
          datasets: [
            {
              label: 'Temperature',
              data: newResult.sensorReadings.map(reading => reading.temperature),
              borderColor: 'rgba(75, 192, 192, 1)',
              fill: false
            },
            {
              label: 'Humidity',
              data: newResult.sensorReadings.map(reading => reading.humidity),
              borderColor: 'rgba(153, 102, 255, 1)',
              fill: false
            }
          ]
        };
      }
    });

    const navigateToForm = () => {
      router.push('/sensor-form');
    };
  
    return {
      pageSize,
      pageNumber,
      dateFrom,
      dateTo,
      chartData,
      chartOptions,
      reQuery,
      navigateToForm
    };
  }
});
</script>
  