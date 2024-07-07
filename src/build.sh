#!/bin/sh

docker build -t latex-base .

cd LatexView

docker compose up --build