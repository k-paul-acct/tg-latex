#!/usr/bin/env bash

export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

docker build -t latex-base:latest .
docker compose build --build-arg="BUILD_CONFIGURATION=Debug"

docker compose down --remove-orphans
docker compose up -d
