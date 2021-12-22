# Creating a Simple IoT Project

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

- add the following libraries that are required for the mqtt connection, a better logging, the json-to-from-object convertor (POJO) and processing the configuration file.

```
dotnet add package MQTTnet --version 3.1.1
dotnet add package MQTTnet.Extensions.ManagedClient --version 3.1.1
dotnet add package serilog
dotnet add package Newtonsoft.Json
dotnet add package Serilog.Sinks.Console
dotnet add package System.Configuration.ConfigurationManager --version 6.0.0
```
- App.config: Create an App.config file and edit it as follow. The brokerUrl can be replaced with localhost if it will tested seperately.  The first three parameters are the standard mqttp parameters, the rest defines how the simulator should send the data to the mqtt broker. Below, `waiting_daruratio` represents the data transmission frequency, while `sim_mode_changing_duration` indicates how much time the broker should pause, i.e. no data transmission. The last parameter shows the sensor UUID, which differs this sensor from others.

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
- Classes: Simulator class describes how often a random value should be generated and transmitted, and then pause. It can be extended and controlled from another script through the app.config file

- Run the program to see how it works. The minimum requirements for the test is to have an mqtt broker and a tool that can behave as subscriber. Both are given in the MQTT-Broker section.

`dotnet run`

- Dockerize: The docker file describes how the project will be dockerized, i.e. copying the files, restoring the app, building/publishing the program as a release. Notice that the multi-state build is applied in order to keep docker container small and efficient. Multi-state docker building works simply as follows: the requirement on the first image will not moved to the second image, so that the images will kept small sized. Please the docker-compose section, where the references to this topic are given.

#### Data Processing&Observer Service (Temperature Observer Service / Data Consumer)

This module consumes the data sent from the temperature sensor and process it. The actual service sends all received objects to the MQTT broker as well as error messages if any message is received for a while.

**Technical View**

`dotnet new console --name iotservice`

- App.config: Same structure as in iotsensor.
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
- Classes:
    - Consumer.cs file is the main class and initiates to receive the messages using Servies/MQTTConnection object and seperatly listens in a distinct loop for understanding whether the messages can be received in a max amount of time, if not the a message is sent to the broker reporting this issue.

- Run the program to see how it works. The minimum requirements for the test is to have an mqtt broker and a tool that can behave as publisher. Both are given in the MQTT-Broker section.
 
`dotnet run`

- Dockerize: Same structed as in iotsensor. 

#### Mongo Database

One of the project goals is to make the data persistent in a database. In this project, the mongodb is chosen to save the data. For the aforementioned use-ase any kind of database either sql or no-sql would fulfill the requirements. Another reason to select the mongodb is the availability of welld structured and written tutorials and documents in this link https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-5.0&tabs=visual-studio or many other links.

**Technical View**
- Configuration: All database configurations are written in the docker-compose file. If the intention is to test it locally, the image can be initiated via docker with the configuration parameters givne in the docker-compose file.
- mongodb-settings/mongo-init.js: Whenever the mongo database is started, it calls this javascript script and creates a database with the name `iotworld`, user `root` and pwd `universe`. These parameters must be given 
- 
#### Data Storing&Requesting Service 

All sensor data is directly stored in the mongo database and the swagger interface enables to fetch and visualize the stored data from the database. The view of the swagger interface is depicted below.

![alt text](https://github.com/cemakpolat/simple-iot-project/blob/master/docs/swagger.png)

**Technical View**
- create project:  This module differs from the iotsensor and iotservice due to its requirements covering the database and its visualization. For this reason, a different project model `webapi` is selected to support the MVC concept. This link gives a detailled view to the this approach: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-5.0&tabs=visual-studio for the mac environment. Apart from the MVC part, if a message comes from the MQTT Broker, it is directly stored in the database.

`dotnet new webapi --name datastore`

- Add the following libraries: The libraries are necessary for interacting with the mongodb database, communicating via MQTT broker, a better logging and supporting swagger.
```
dotnet add package MongoDB.Driver
dotnet add package MQTTnet --version 3.1.1
dotnet add package MQTTnet.Extensions.ManagedClient --version 3.1.1
dotnet add package serilog
dotnet add package Newtonsoft.Json
dotnet add package Serilog.Sinks.Console
dotnet add package serilog --version 2.10.0
dotnet add package Swashbuckle.AspNetCore --version 5.6.3
```

- appsettings.json: This file is automatically created and it is also a configuration file like App.config, just the notations are in JSON rather than XML. Three configuration blocks are available: 1) Logging config, 2) Database settings such as user name, password and database name, these can be directly passed from the docker-compose, 3) mqtt settings provided by the docker-compose. If the goal is to test only this component, modify the the brokerUrl address such as localhost or another url address.
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "DatabaseSettings": {
    "ConnectionString": "mongodb://root:universe@mongodb:27017/?readPreference=primary&ssl=false",
    "Database": "iotworld"
  },
  "MQTTSettings":{
    "BrokerURL":"broker",
    "BrokerPort":"1883",
    "MQTTClientID":"datastore-service"
  },
  "AllowedHosts": "*"
}
```
- launchsettings.json: This file is also created and it includes the launch settings. Two parameters are important, `applicationUrl` should be similar what is written in docker-compose file. The initial configuration has localhost:5000 as a value. `ASPNETCORE_ENVIRONMENT` is also important and must be given in docker-compose. Otherwise the swagger interface cannot be accesed.
- Classes
    - Controller/IoTEntityController.cs is a typical rest class, which enables user to send/receive the request/response. In case of need, this rest interface can be extended.
    - Domain/* classes shows the structure how the data will be stored in the mongodb. If 
    - Models/* classes are used to convert the Json messages to the classic objects, which are shown also in other examples
    - Services/* supports the classes direct interaction with the database using IoTService and mqtt connection via MQTTConnection. The IoT object in JSON lands in the MQTTConnection object, converted to Entity object and then stored in the mongodb using IoTService object
    - Startup.cs class behaves like main initiater, adds all required app configuration, append new classes such as IoTservice and initiate the MQTT connection.
- Dockerfile: Same structure as in iotsensor


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
