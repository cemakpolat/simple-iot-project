FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /app
COPY iotsensor.csproj ./
RUN dotnet restore
COPY . ./

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "iotsensor.dll"]