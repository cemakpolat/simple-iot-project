# Building a Simple IoT middleware

## Use Case 

A smart factory environment is composed of many simple and complex devices that owns diverse features. The harmonization of all these features necessitates an IoT platform that unifies distinguished data models ranging from devices to human actor. The well known topic and crucial aspect of the smart factories is to predict beforehand when a device will be broken, since the whole production chain can severely be affected from a broken device in the system, and this may lead to stopping the complete system, which is not desired due to the cost and user perspectives. In order to prevent this kind of issues, a predictive maintenance system should be introduced to foresee when a device will be damaged, and it has to be replaced. To visualize such a use-case, we assume that a temperature sensor embedded in the production line is continuously monitored in a factory environment. All data retrieved from this sensor to the backend services for further analysis. Based on the observation analysis, the possibility of having a broken device is predicted. In the following section, only the data aggregation and visualization parts are implemented. The predictive maintenance module will be later integrated in the rest of the system.


## System Architecture 

The realization of this idea can be performed via a small IoT setup presented below. 
![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/arch.png)

### Required Technologies

- Communication Protocols: MQTT, HTTP, Websocket 
- Programming Language: C#, JavaScript, TypeScript, Python
- Database: MongoDB
- Container Tech: Docker
- Machine Learning Algorithm: LSTM, … (TODO)
- Development Environment: Mac OSX Catalina
- .NET 5: Install from this link https://docs.microsoft.com/en-us/dotnet/core/install/macos

### System Components 

#### MQTT Broker

The data distribution among all other components are carried out via MQTT broker. In this project, the selected mqtt-broker is customized image based on the mosquitto-eclipse broker `toke/mosquitto`, because the websocket communication is enabled in this image, which is required for the user interface.

**Technical View**
You can test the broker with the following command line:

`docker run -ti -p 127.0.0.1:1883:1883 -p 9001:9001 toke/mosquitto`

The easiest way to test the broker either run a simple python script or using a tool such as MQTT.fx. For python libraries, you can check this link: https://github.com/emqx/MQTT-Client-Examples/tree/master/mqtt-client-Python3

#### IoT Data Model 
 
 An IoT data model abstracts the features of an IoT sensor and allow its communication with other sensors or devices. We (me and other colleagues) described an IoT model in https://ieeexplore.ieee.org/document/8767276 and implemented it in https://github.com/GT-ARC/chariot-apis. In this tutorial, a very simplified version of this model is utilized. Two classes, namely, IoTEntity and Property belonging to the IoTEntity are implemented with the limited features. This model covers only the primive devices, for a complex device, it is recommended to utilize the whole library.

#### Common-API

The desired approach of using an IoT data model is to describe it under a common-api folder and enable its integration by other projects. Even this approach is possible and described how to do in common-api/README.md file, it is not considered in this tutorial.

#### Temperature Sensor/Service (Producer):

Sensor simulates a temperature service that generates randomly temperature values and sends it through MQTT communication protocol to the broker as a publisher. The service simulating device first transmits data and then stops for a while in a continous loop. The behavior of the simulation can be easily modified using the `App.config` file. 

**Technical View**

- create a project called iotservice:

`dotnet new console --name iotservice`

- add the required libraries:

```
dotnet add package MongoDB.Driver
dotnet add package MQTTnet --version 3.1.1
dotnet add package serilog
dotnet add package Newtonsoft.Json
dotnet add package Serilog.Sinks.Console
```
- Create an App.config file and edit it as follow. The brokerUrl can be replaced with localhost if it will tested seperately.  The first three parameters are the standard mqttp parameters, the rest defines how the simulator should send the data to the mqtt broker. Below, `waiting_daruratio` represents the data transmission frequency, while `sim_mode_changing_duration` indicates how much time the broker should pause, i.e. no data transmission. The last parameter shows the sensor UUID, which differs this sensor from others.
- 
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="brokerUrl" value="broker" />
        <add key="brokerPort" value="1883" />
        <add key="mqttClientId" value="sensor-1" />
        <add key="waiting_duration" value="100" />
        <add key="sim_mode_changing_duration" value="10000" />
        <!-- <add key="entityId" value="UUID" /> -->
    </appSettings>
</configuration>
```
- Classes
- Dockerfile
dotnet run 
#### Data Processing&Observer Service (Temperature Observer Service / Data Consumer)

This module consumes the data sent from the temperature sensor and process it. The actual service sends all received objects to the MQTT broker as well as error messages if any message is received for a while.

**Technical View**
`dotnet new console --name iotservice`

- App.config
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="brokerUrl" value="broker" />
        <add key="brokerPort" value="1883" />
        <add key="mqttClientId" value="service-1" />
        <!-- <add key="entityId" value="UUID" /> -->
    </appSettings>
</configuration>
```
- Classes
    - Consumer, Servies/MQTTConnection
- Dockerfile

dotnet run
#### Data Storing&Requesting Service 

All sensor data is directly stored in the mongo database and the swagger interface enables to fetch the stored data from the database.

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/swagger.png)

**Technical View**
create project

`dotnet new webapi --name datastore`

add the required libraries:
```
dotnet add package MongoDB.Driver
dotnet add package MQTTnet --version 3.1.1
dotnet add package serilog
dotnet add package Newtonsoft.Json
dotnet add package Serilog.Sinks.Console
```

- appsettings.json
- launchsettings.json
- Classes
    - MQTTConnection
    - Producer
    - Simulator
- Dockerfile
- mongo-settings

#### User Interfaces

An enduser application is offered to monitor the temperature sensor values and the log messages sent by the services. These interfaces are demonstrated below:

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/sensor.png)

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/log.png)

**Technical View**

The app is constructed on the free vesion of (coreui/angular)[https://coreui.io/angular/] based on the angular framework. 
- important files that should be explained for the clarification of the code structure
- MQTT service over websocket

#### Predictive Maintenance (TODO)

An autonomous smart factory necessitates to detect the errorness or the possibility of having a failure at devices. The predictive maintenance service aims at aggregating the data from the sensors and predict whether the device behaves normal based on the historical data. This component will be added as a future work. 

**Technical View**

#### Common Class Library

Some projects includes the repetitive codes, and this class library targets to eliminate this issue by designing a common architecture. In the current version it is integrated in the other modules. It is highly recommeded to read the common-api/README.md that indicates how to do this approach.

## Docker-Compose Structure

# How to run

All project components are dockerized and docker-compose solves all dependency issues among the programs. To run the project, call simply the following commands:

`docker-compose build`

`docker-compose up`


User interface:

http://localhost:4200/

Swagger interface

http://localhost:5003/swagger/index.html

Rest interface:

http://localhost:5003/api/Entity/

## Dotnet console commands for mac environment

https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new

dotnet new Console --framework net6.0
dotnet new console -n subscriber
dotnet build
dotnet run
dotnet restore
dotnet update

dotnet add package MongoDB.Driver
dotnet add package MQTTnet --version 3.1.1
dotnet add package serilog
dotnet add package Newtonsoft.Json
dotnet add package Serilog.Sinks.Console
