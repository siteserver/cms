FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /sscms
COPY . .
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_12.x | bash \
    && apt-get install nodejs -yq
RUN npm install
RUN npm run build-linux-x64
RUN dotnet build ./build-linux-x64/build.sln -c Release

FROM build AS publish
RUN dotnet publish ./build-linux-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -c Release -o ./publish/sscms-linux-x64
RUN dotnet publish ./build-linux-x64/src/SSCMS.Web/SSCMS.Web.csproj -c Release -o ./publish/sscms-linux-x64
RUN npm run copy-linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /sscms/publish/sscms-linux-x64 .
ENTRYPOINT ["dotnet", "SSCMS.Web.dll"]