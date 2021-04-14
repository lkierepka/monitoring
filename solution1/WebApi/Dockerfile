FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app
COPY WebApi/*.csproj ./WebApi/
COPY WorkerService.Contracts/*.csproj ./WorkerService.Contracts/
COPY ./external/opentelemetry-dotnet-contrib/src/OpenTelemetry.Contrib.Instrumentation.MassTransit/*.csproj /app/external/opentelemetry-dotnet-contrib/src/OpenTelemetry.Contrib.Instrumentation.MassTransit/
WORKDIR /app/WebApi
RUN dotnet restore

COPY ./WebApi /app/WebApi/
COPY ./WorkerService.Contracts /app/WorkerService.Contracts/
COPY ./external /app/external/
RUN dotnet build -c Release -maxCpuCount
RUN dotnet publish -c Release -maxCpuCount -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]