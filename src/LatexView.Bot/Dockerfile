FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet nuget add source https://pkgs.dev.azure.com/tgbots/Telegram.Bot/_packaging/release/nuget/v3/index.json -n Telegram.Bot
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY src/LatexView.Bot/LatexView.Bot.csproj src/LatexView.Bot/
RUN dotnet restore src/LatexView.Bot/LatexView.Bot.csproj
COPY . .
WORKDIR /src/src/LatexView.Bot
RUN dotnet build LatexView.Bot.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish LatexView.Bot.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LatexView.Bot.dll"]
