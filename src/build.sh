#!/usr/bin/env bash

docker build -t latex-base .

cd LatexView

docker compose build