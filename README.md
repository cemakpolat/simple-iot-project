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
1. Sensor Service
2. Data Processing&Observer Service
3. Data Storing&Requesting Service
4. User Interface
5. Predictive Maintenance (TODO)


# How to run
To run the project, call simply the following commands:

`docker-compose build`

`docker-compose up`
