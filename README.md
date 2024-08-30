# Raspberry Pi Sensor + Chart display (C#)
I have created this project as a showcase of what can be done with C#, dot net , an old Raspberry pi and Angular
This project has several parts separated into each one of the folders as described below.

## RPISensor
This folder contains the C# microservice. This project is meant to be ran in a raspberry pi, with a DHT22 sensor connected to pin 26 (pin number can be set up from appSettings.json). This should also work with a DHT11 Sensor. If your DHT22 comes with 4 pins,  remember to build the schematic  with a 10k ohm resistance between the data pin and VCC. If however it comes with 3 pins and sodered to a small motherboard then the resistance is already in place and you just need to connect it to VCC, GND and whatever pin you want to use. You can google this setup, it is very common.

## APITests
This folder contains the integration tests for the controller and the database service. They are meant to be ran in any machine within any CI/CD

## RPISensorTests
This folder contains the tests that ensure the DHT22 sensor is working, and are only meant to be executed in the Raspberry pi. This way you can include your raspberry pi tests in your final CI/CD, but your build agent / pipeline must have access to the RPI

## ops
The operations folder contains all the necessary information to set up a new database. This is used by docker compose to generate a new instance of a database so you can test and try things out. Use
docker compose up

To launch a local mariadb database that your project can use to work and develop. Integration tests use the built-in memory sql database that comes with DotNet, but you can change this setup to use a Real Database if you want to enforce the whole data pipeline rather than mocking the database to an in memory (which is much more comfortable in CI/CD, but will not handle database creation errors in staging or production).
> **NOTE**: Docker relies on .env file to create this, so make sure you copy .env.dist into .env

## RPIDHT22Driver
This folder contains the C driver that the application uses to read temperature and humidity from its sensors

## Setting up Raspberry pi
This project has been built and created using Raspbian os. 
The settings file can be found in appsettings.dist. File must be copied into appsettings.json and appsettings.Development.json inside of both folders SensorDataAPI and SensorDHT22 . You may then change the parameters as needed.

> **NOTE**: You must install and compile wiring-pi [here](https://github.com/WiringPi/WiringPi). 

You may download their latest release from their Prebuilt binaries or build it yourselrf running the following command wherever you feel like installing WiringPi library. You'll also need the build-essential linux package.
```
sudo apt update
sudo apt install build-essential
git clone https://github.com/WiringPi/WiringPi.git
cd WiringPi
./build
```

Once this is done, you'll need to compile the driver
```
cd  RPIDHTDriver
gcc -o rptidht22driver rpidht22driver.c -lwiringPi
```
The application then runs the command ./rpidht22driver <GPIO pin_number>, the home made driver will return two numbers as Temperature and Humidity. The C# application uses this command to read the data


## Setting up the C# endpoint
Move the  appsettings.dist into the SensorDataAPI folder. the navigate into that folder and run dotnet run for development or dotnet publish for production / debug releases

