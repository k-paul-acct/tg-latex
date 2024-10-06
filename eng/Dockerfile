FROM debian:12-slim AS latex

RUN apt update -y
RUN apt upgrade -y

RUN apt install -y texlive-full
RUN apt install -y texlive-latex-extra

FROM latex AS dotnet

USER root

RUN apt install -y wget
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb

RUN apt update -y
RUN apt upgrade -y

RUN apt install -y aspnetcore-runtime-8.0
