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

### System Components 

#### MQTT Broker

The data distribution among all other components are carried out via MQTT broker.


#### Common Lass Lib
How to create a class library and how it can be consumed by another app
dotnet new classlib
dotnet pack
dotnet add reference ../common-api/common-api.csproj
Link: https://stackoverflow.com/questions/49805204/adding-reference-to-another-project-from-visual-studio-code
https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

#### Temperature Sensor/Service (Producer):

Sensor simulates a temperature service that generates randomly temperature values and sends it through MQTT communication protocol to the broker as a publisher. The device simulating service first transmits data and then stops for a while in a continous loop. 

**Technical View**
Configuration File

#### Data Processing&Observer Service (Temperature Observer Service / Data Consumer)

This module consumes the data sent from the temperature sensor and process it. The actual service sends all received objects to the MQTT broker as well as error messages if any message is received for a while.

**Technical View**

#### Data Storing&Requesting Service 

All sensor data is directly stored in the mongo database and the swagger interface enables to fetch the stored data from the database.

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/swagger.png)

**Technical View**

#### User Interfaces

An enduser application is offered to monitor the temperature sensor values and the log messages sent by the services. These interfaces are demonstrated below:

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/sensor.png)

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/log.png)

**Technical View**

The app is constructed on (coreui/angular)[https://coreui.io/angular/] based on the angular framework.

#### Predictive Maintenance (TODO)

An autonomous smart factory necessitates to detect the errorness or the possibility of having a failure at devices. The predictive maintenance service aims at aggregating the data from the sensors and predict whether the device behaves normal based on the historical data. This component will be added as a future work. 

**Technical View**


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

## Dotnet consoel commands for mac environment

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