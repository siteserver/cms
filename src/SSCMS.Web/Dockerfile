#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

WORKDIR /src

# Install NodeJS 10
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq

RUN npm install
RUN npm run build

COPY ["src/SSCMS.Web/SSCMS.Web.csproj", "src/SSCMS.Web/"]
RUN dotnet restore "src/SSCMS.Web/SSCMS.Web.csproj"
COPY . .
WORKDIR "/src/src/SSCMS.Web"
RUN dotnet build "SSCMS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSCMS.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SSCMS.Web.dll"]