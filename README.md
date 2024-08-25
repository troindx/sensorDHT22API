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
And then run the command ./rpidht22driver <GPIO pin_number>, the home made driver will return two numbers. The C# application uses this command to read the data
The driver must be ran with sudo priviledges in the raspberry pi, in order for it to have full access to the GPIO pins
