FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app
COPY WebApi/*.csproj ./WebApi/
COPY WorkerService.Contracts/*.csproj ./WorkerService.Contracts/
WORKDIR /app/WebApi
RUN dotnet restore

COPY . /app/
RUN dotnet publish -c Release -maxCpuCount -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]