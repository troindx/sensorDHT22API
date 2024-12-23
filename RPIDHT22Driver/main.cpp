#include <iostream>
#include <cstdlib>
#include "dht.h"

int main(int argc, char* argv[]) {
    if (argc != 2) {
        std::cerr << "Usage: " << argv[0] << " <pin_number>" << std::endl;
        return 1;
    }

    int pin = std::atoi(argv[1]);
    if (pin <= 0) {
        std::cerr << "Invalid pin number: " << argv[1] << std::endl;
        return 1;
    }

    dht sensor;
    int result = sensor.read22(pin);

    if (result == DHTLIB_OK) {
        std::cout << sensor.temperature << ";" << sensor.humidity << std::endl;
    } else {
        std::cerr << "Error reading sensor: ";
        switch (result) {
            case DHTLIB_ERROR_CHECKSUM: std::cerr << "Checksum error"; break;
            case DHTLIB_ERROR_TIMEOUT: std::cerr << "Timeout error"; break;
            case DHTLIB_ERROR_CONNECT: std::cerr << "Connect error"; break;
            case DHTLIB_ERROR_ACK_L: std::cerr << "ACK Low error"; break;
            case DHTLIB_ERROR_ACK_H: std::cerr << "ACK High error"; break;
            default: std::cerr << "Unknown error"; break;
        }
        std::cerr << std::endl;
        return 1;
    }

    return 0;
}
