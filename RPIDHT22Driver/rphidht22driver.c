#include <wiringPi.h>
#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>

#define MAX_TIMINGS 85
#define RETRY_LIMIT 5

int data[5] = {0, 0, 0, 0, 0};

int read_dht_data(int pin, float *temperature, float *humidity)
{
    uint8_t laststate = HIGH;
    uint8_t counter = 0;
    uint8_t j = 0, i;

    data[0] = data[1] = data[2] = data[3] = data[4] = 0;

    // Pull pin down for 18 milliseconds
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW);
    delay(18);

    // Then pull it up for 50 microseconds (increase from 40 to 50)
    digitalWrite(pin, HIGH);
    delayMicroseconds(50);

    // Prepare to read the pin
    pinMode(pin, INPUT);

    // Detect change and read data
    for (i = 0; i < MAX_TIMINGS; i++)
    {
        counter = 0;
        while (digitalRead(pin) == laststate)
        {
            counter++;
            delayMicroseconds(2); // Increase delay between checks (from 1 to 2)
            if (counter == 255)
            {
                break;
            }
        }
        laststate = digitalRead(pin);

        if (counter == 255)
            break;

        // Ignore first 3 transitions
        if ((i >= 4) && (i % 2 == 0))
        {
            // Shift in data
            data[j / 8] <<= 1;
            if (counter > 16)
                data[j / 8] |= 1;
            j++;
        }
    }

    // Check if we received 40 bits and the checksum is correct
    if ((j >= 40) && (data[4] == ((data[0] + data[1] + data[2] + data[3]) & 0xFF)))
    {
        *humidity = (float)((data[0] << 8) + data[1]) / 10;
        if (*humidity > 100)
        {
            *humidity = data[0]; // for DHT11
        }
        *temperature = (float)(((data[2] & 0x7F) << 8) + data[3]) / 10;
        if (*temperature > 125)
        {
            *temperature = data[2]; // for DHT11
        }
        if (data[2] & 0x80)
        {
            *temperature = -*temperature;
        }

        return 1; // Success
    }
    else
    {
        return 0; // Failure
    }
}

int main(int argc, char *argv[])
{
    if (argc != 2)
    {
        printf("Usage: %s <GPIO Pin Number>\n", argv[0]);
        return 1;
    }

    int pin = atoi(argv[1]);

    if (wiringPiSetupGpio() == -1)
    {
        printf("WiringPi setup failed!\n");
        return 1;
    }

    float temperature, humidity;
    int success = 0;

    for (int attempt = 0; attempt < RETRY_LIMIT; attempt++)
    {
        delay(2000); // Wait 2 seconds between retries
        success = read_dht_data(pin, &temperature, &humidity);
        if (success)
        {
            printf("%.1f;%.1f\n", temperature, humidity);
            break;
        }
    }

    if (!success)
    {
        printf("Failed to read from DHT22 sensor after %d attempts\n", RETRY_LIMIT);
        return 1;
    }

    return 0;
}
