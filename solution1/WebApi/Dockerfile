FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app
COPY WorkerService.Contracts/*.csproj ./WorkerService.Contracts/
COPY Common/*.csproj ./Common/
COPY WebApi/*.csproj ./WebApi/
WORKDIR /app/WebApi
RUN dotnet restore

COPY ./WorkerService.Contracts /app/WorkerService.Contracts/
COPY ./Common /app/Common/
COPY ./WebApi /app/WebApi/
RUN dotnet build -c Release -maxCpuCount
RUN dotnet publish -c Release -maxCpuCount -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]