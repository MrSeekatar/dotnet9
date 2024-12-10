FROM mcr.microsoft.com/dotnet/aspnet:9.0.0-noble-chiseled AS base
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

ENV ASPNETCORE_URLS="https://*:44300"

EXPOSE 44300

WORKDIR /app
COPY --from=build /app/publish .
COPY aspnetapp.pfx .

ENTRYPOINT ["dotnet", "BoxServerApi.dll"]
