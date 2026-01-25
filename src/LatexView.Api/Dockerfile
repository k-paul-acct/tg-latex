FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY src .
RUN dotnet restore LatexView.Api/LatexView.Api.csproj
RUN dotnet publish LatexView.Api/LatexView.Api.csproj -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update && apt-get install -y imagemagick
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LatexView.Api.dll"]
