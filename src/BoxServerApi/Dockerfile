FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BoxServerApi/BoxServerApi.csproj", "BoxServerApi/"]
COPY ["BoxServerModels/BoxServerModels.csproj", "BoxServerModels/"]
COPY ["BoxServer/BoxServer.csproj", "BoxServer/"]
COPY ["Box.ServiceDefaults/Box.ServiceDefaults.csproj", "Box.ServiceDefaults/"]
RUN dotnet restore "BoxServerApi/BoxServerApi.csproj"

COPY . .
WORKDIR "/src/BoxServerApi"
RUN dotnet publish "BoxServerApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
COPY --from=build src/cert.pfx /app/cert.pfx

ENV ASPNETCORE_URLS="https://*:44300"

EXPOSE 44300

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BoxServerApi.dll"]
