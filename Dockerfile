#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src
COPY ./BotDash/BotDash.csproj BotDash/
#COPY ./ClassLibrary/ClassLibrary.csproj .

RUN dotnet restore BotDash/BotDash.csproj
COPY . .
WORKDIR /src/BotDash
RUN dotnet build "BotDash.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BotDash.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BotDash.dll"]