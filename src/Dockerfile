FROM ubuntu AS latex

RUN apt update -y
RUN apt upgrade -y

RUN apt install -y texlive-full
RUN apt install -y texlive-latex-extra

FROM latex AS dotnet

RUN apt update -y
RUN apt upgrade -y

RUN apt install -y dotnet-sdk-8.0

FROM dotnet AS update

RUN apt update -y
RUN apt upgrade -y