This module serves the usage of common definitions to other libraries. Instead of repeating the same code blocks in different code blocks, i.e. iotsensor, iotservice and datastore do include IoTentity and Property classes, this seperate module can be directly involved into them. As the docker-compose and dockerfile are used in the whole project, involving this class library to others, makes the management of the docker file complicated. A better approach is to upload this class library to the online nuget repository and add it as a package. As a result, I prefer to not integrate this library into the other projects. Nevertheless, two solutions considering two diffrent cases are given below:

1- Dockerize web app with dependent class library

- https://imranarshad.com/dockerize-net-core-web-app-with-dependent-class-library/
- https://stackoverflow.com/questions/64557885/how-to-include-class-library-reference-into-docker-file

2- Create and publish a package

- https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli

It is also recommended to read the 
- Containerize your .NET Core app – the right way [https://medium.com/01001101/containerize-your-net-core-app-the-right-way-35c267224a8d],
- Multi-project .net core solution in Docker image [https://www.softwaredeveloper.blog/multi-project-dotnet-core-solution-in-docker-image]
pages to understand the overall docker concept for the dot net apps.

# How to reference this class lib

- `cd target_project_folder/`
- `dotnet add reference ../common-api/common-api.csproj`
