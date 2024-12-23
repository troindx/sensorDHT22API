//
//    FILE: dht.cpp
//  AUTHOR: Rob Tillaart
// VERSION: 0.1.18
// PURPOSE: DHT Temperature & Humidity Sensor library for Raspberry Pi
//
// HISTORY:
// See original comments for history details.
//
#include "dht.h"
#include <wiringPi.h>
#include <stdint.h>
#define min(a, b) ((a) < (b) ? (a) : (b))

/////////////////////////////////////////////////////
//
// PUBLIC
//
int dht::read11(uint8_t pin) {
    // READ VALUES
    int result = _readSensor(pin, DHTLIB_DHT11_WAKEUP, DHTLIB_DHT11_LEADING_ZEROS);

    // these bits are always zero, masking them reduces errors.
    bits[0] &= 0x3F;
    bits[2] &= 0x3F;

    // CONVERT AND STORE
    humidity = bits[0];  // bits[1] == 0;
    temperature = bits[2];  // bits[3] == 0;

    // TEST CHECKSUM
    uint8_t sum = bits[0] + bits[2];
    if (bits[4] != sum) {
        return DHTLIB_ERROR_CHECKSUM;
    }
    return result;
}

int dht::read(uint8_t pin) {
    // READ VALUES
    int result = _readSensor(pin, DHTLIB_DHT_WAKEUP, DHTLIB_DHT_LEADING_ZEROS);

    // these bits are always zero, masking them reduces errors.
    bits[0] &= 0x03;
    bits[2] &= 0x83;

    // CONVERT AND STORE
    humidity = ((bits[0] << 8) | bits[1]) * 0.1;
    temperature = (((bits[2] & 0x7F) << 8) | bits[3]) * 0.1;  // Corrected `word` usage
    if (bits[2] & 0x80) {  // negative temperature
        temperature = -temperature;
    }

    // TEST CHECKSUM
    uint8_t sum = bits[0] + bits[1] + bits[2] + bits[3];
    if (bits[4] != sum) {
        return DHTLIB_ERROR_CHECKSUM;
    }
    return result;
}

/////////////////////////////////////////////////////
//
// PRIVATE
//
int dht::_readSensor(uint8_t pin, uint8_t wakeupDelay, uint8_t leadingZeroBits) {   
    uint8_t mask = 128;
    uint8_t idx = 0;

    uint8_t data = 0;
    uint8_t state = LOW;
    uint8_t pstate = LOW;
    uint16_t zeroLoop = DHTLIB_TIMEOUT;
    uint16_t delta = 0;

    leadingZeroBits = 40 - leadingZeroBits; // reverse counting...

    // REQUEST SAMPLE
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW); // T-be
    delay(wakeupDelay);
    digitalWrite(pin, HIGH); // T-go
    pinMode(pin, INPUT);

    uint16_t loopCount = DHTLIB_TIMEOUT * 2;  // 200uSec max
    while (digitalRead(pin) == HIGH) {  // Wait for sensor response
        if (--loopCount == 0) return DHTLIB_ERROR_CONNECT;
    }

    // GET ACKNOWLEDGE or TIMEOUT
    loopCount = DHTLIB_TIMEOUT;
    while (digitalRead(pin) == LOW) {  // T-rel
        if (--loopCount == 0) return DHTLIB_ERROR_ACK_L;
    }

    loopCount = DHTLIB_TIMEOUT;
    while (digitalRead(pin) == HIGH) {  // T-reh
        if (--loopCount == 0) return DHTLIB_ERROR_ACK_H;
    }

    loopCount = DHTLIB_TIMEOUT;

    // READ THE OUTPUT - 40 BITS => 5 BYTES
    for (uint8_t i = 40; i != 0; ) {
        // WAIT FOR FALLING EDGE
        state = digitalRead(pin);
        if (state == LOW && pstate != LOW) {
            if (i > leadingZeroBits) { // DHT22 first 6 bits are all zero !! DHT11 only 1
                zeroLoop = min(zeroLoop, loopCount);
                delta = (DHTLIB_TIMEOUT - zeroLoop) / 4;
            } else if (loopCount <= (zeroLoop - delta)) { // long -> one
                data |= mask;
            }
            mask >>= 1;
            if (mask == 0) {  // next byte
                mask = 128;
                bits[idx] = data;
                idx++;
                data = 0;
            }
            --i;  // next bit

            loopCount = DHTLIB_TIMEOUT;  // Reset timeout flag
        }
        pstate = state;

        // Check timeout
        if (--loopCount == 0) {
            return DHTLIB_ERROR_TIMEOUT;
        }
    }
    pinMode(pin, OUTPUT);
    digitalWrite(pin, HIGH);

    return DHTLIB_OK;
}
