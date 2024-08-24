## Setting up Raspberry pi
This project has been built and created using Raspbian os. 

> **NOTE**: Documentation for DHTXX can be found [here](https://github.com/dotnet/iot/blob/main/src/devices/Dhtxx/README.md). Also further documentation for using MS IoT Core instead of Raspbian.

The project uses the libgpiod2 drivers so in raspberry cli run the following commands.

```
sudo apt update
sudo apt upgrade (if neccesary)
sudo apt install libgpiod2
```
