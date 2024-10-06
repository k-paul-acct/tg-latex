#!/usr/bin/env bash

export ASPNETCORE_ENVIRONMENT=Production
export DOTNET_ENVIRONMENT=Production

docker build -t latex-base:latest .
docker compose build --build-arg="BUILD_CONFIGURATION=Release"

docker compose down --remove-orphans
docker compose up -d
