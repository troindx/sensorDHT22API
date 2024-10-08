<template>
  <h1>Insert manually the information</h1>
    <v-container>
      <v-form @submit.prevent="onSubmit">
        <v-text-field
          v-model="temperature"
          label="Temperature"
          type="number"
          required
        ></v-text-field>
        <v-text-field
          v-model="humidity"
          label="Humidity"
          type="number"
          required
        ></v-text-field>
        <v-btn type="submit" color="primary">Submit</v-btn>
      </v-form>
    </v-container>
  </template>
  
<script lang="ts">
import { defineComponent, ref } from 'vue';
import { useMutation } from '@vue/apollo-composable';
import { gql } from '@apollo/client/core';
import { useRouter } from 'vue-router';
import { formatDateToBackend } from '@/util/dateUtils';
// Define your mutation
const CREATE_SENSOR_READING = gql`
   mutation createSensorReading($input: CreateSensorReadingInput!) {
      createSensorReading(input: $input){
       sensorReading {
        humidity
        time
        temperature
        id
       }
       code
       message
      }
    }

  `;
  
export default defineComponent({
  setup() {
    const temperature = ref(0);
    const humidity = ref(0);
    const router = useRouter();
  
    const { mutate: createSensorReading } = useMutation(CREATE_SENSOR_READING);
  
    const onSubmit = async () => {
      try {
        await createSensorReading({
          input: {
            temperature: parseFloat(temperature.value.toString()),
            humidity: parseFloat(humidity.value.toString()),
            time: formatDateToBackend(new Date())
          }
        });
        router.back();
      } catch (error) {
        console.error('Error submitting sensor reading:', error);
      }
    };
  
    return {
      temperature,
      humidity,
      onSubmit
    };
  }
});
</script>
  