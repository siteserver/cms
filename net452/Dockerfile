FROM microsoft/aspnet:4.7

RUN powershell -NoProfile -Command Remove-Item -Recurse C:\inetpub\wwwroot\*

WORKDIR /inetpub/wwwroot

COPY Dockerfile.ps1 .

RUN powershell -executionpolicy bypass .\Dockerfile.ps1

COPY build/ .