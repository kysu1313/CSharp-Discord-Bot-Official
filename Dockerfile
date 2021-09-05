
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DiscBotConsole/DiscBotConsole.csproj", "DiscBotConsole/"]
RUN dotnet restore "DiscBotConsole/DiscBotConsole.csproj"

COPY ["DiscBotConsole2/BotDashboard.csproj", "DiscBotConsole2/"]
RUN dotnet restore "DiscBotConsole2/BotDashboard.csproj"

COPY . .
WORKDIR "/src/DiscBotConsole"
RUN dotnet build "DiscBotConsole.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscBotConsole.csproj" -c Release -o /app/publish
ENV ASPNETCORE_ENVIRONMENT Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "DiscBotConsole.dll"]
