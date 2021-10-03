
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ../DiscBotConsole.sln ./
COPY ["DiscBotConsole/DiscBotConsole.csproj", "DiscBotConsole/"]
COPY ["BotApi/BotApi.csproj", "BotApi/"]
COPY ["BotDash/BotDash.csproj", "BotDash/"]
COPY ["ClassLibrary/ClassLibrary.csproj", "ClassLibrary/"]
COPY ["BusinessLogic/BusinessLogic.csproj", "BusinessLogic/"]

# "DiscBotConsole/DiscBotConsole.csproj"
RUN dotnet restore 
COPY . .
WORKDIR "/src/DiscBotConsole"
RUN dotnet build "DiscBotConsole.csproj" -c Release -o /app

WORKDIR "/src/BotApi"
RUN dotnet build "BotApi.csproj" -c Release -o /app

WORKDIR "/src/BotDash"
RUN dotnet build "BotDash.csproj" -c Release -o /app
#FROM build AS publish
#RUN dotnet publish "DiscBotConsole.csproj" -c Release -o /app/publish
#ENV ASPNETCORE_ENVIRONMENT Development

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "DiscBotConsole.dll"]
