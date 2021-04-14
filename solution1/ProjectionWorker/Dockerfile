FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app
COPY ProjectionWorker/*.csproj ./ProjectionWorker/
COPY WorkerService.Contracts/*.csproj ./WorkerService.Contracts/
COPY ./external/opentelemetry-dotnet-contrib/src/OpenTelemetry.Contrib.Instrumentation.MassTransit/*.csproj /app/external/opentelemetry-dotnet-contrib/src/OpenTelemetry.Contrib.Instrumentation.MassTransit/
WORKDIR /app/ProjectionWorker
RUN dotnet restore

COPY ./ProjectionWorker /app/ProjectionWorker/
COPY ./WorkerService.Contracts /app/WorkerService.Contracts/
COPY ./external /app/external/
RUN dotnet publish -c Release -maxCpuCount -o /app/out

FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProjectionWorker.dll"]