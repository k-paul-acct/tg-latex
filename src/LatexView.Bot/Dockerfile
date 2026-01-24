FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src/LatexView.Bot .
RUN dotnet restore LatexView.Bot.csproj
RUN dotnet publish LatexView.Bot.csproj -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LatexView.Bot.dll"]
