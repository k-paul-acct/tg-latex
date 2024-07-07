FROM latex-base AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY LatexView.Api/LatexView.Api.csproj LatexView.Api/
RUN dotnet restore LatexView.Api/LatexView.Api.csproj
COPY . .
WORKDIR /src/LatexView.Api
RUN dotnet build LatexView.Api.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish LatexView.Api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet LatexView.Api.dll