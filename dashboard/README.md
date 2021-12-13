# angular-mqtt
Dead simple angular mqtt example using mosquitto local broker

For enabling mosquitto to send over websockets

listener 9001
protocol websockets

# Dockerize
https://softchris.github.io/pages/dotnet-dockerize.html#clean-up
https://docs.docker.com/samples/dotnetcore/


# Frontend

https://github.com/ngodup/angular-chartjs/tree/main/src/app

https://www.chartjs.org/docs/latest/
https://github.com/anantl05/angular-mqtt
https://medium.com/@anant.lalchandani/dead-simple-mqtt-example-over-websockets-in-angular-b9fd5ff17b8e
https://www.positronx.io/angular-chart-js-tutorial-with-ng2-charts-examples/
https://tudip.com/blog-post/how-to-implement-chart-js-in-angular/
https://andrew-flower.com/blog/angular-chartjs

# Backend

https://github.com/chkr1011/MQTTnet/issues/1040
https://github.com/SeppPenner/SimpleMqttServer/tree/master/src
https://github.com/chkr1011/MQTTnet/wiki/Client
https://github.com/EduardoJuliao/MQTTFirstLook/blob/main/MQTTFirstLook.Client/Program.cs
https://dev.to/eduardojuliao/basic-mqtt-with-c-1f88

dotnet add package MQTTnet --version 3.1.1

docker run -ti -p 127.0.0.1:1883:1883 -p 9001:9001  toke/mosquitto

https://github.com/chkr1011/MQTTnet/blob/master/Tests/MQTTnet.Core.Tests/Client/MqttClient_Tests.cs

# Dockerize Angular App
https://www.section.io/engineering-education/containerizing-an-angular-app-featuring-nginx-web-server-using-docker/
