#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/SSCMS.Web/SSCMS.Web.csproj", "src/SSCMS.Web/"]
COPY ["src/SSCMS.Core/SSCMS.Core.csproj", "src/SSCMS.Core/"]
COPY ["src/SSCMS/SSCMS.csproj", "src/SSCMS/"]
RUN dotnet restore "src/SSCMS.Web/SSCMS.Web.csproj"
COPY . .
WORKDIR "/src/src/SSCMS.Web"
RUN dotnet build "SSCMS.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SSCMS.Web.csproj" -c Release -o /app/sscms
RUN cp -r /app/sscms/wwwroot /app/sscms/_wwwroot
RUN echo "7.0.10" > /app/sscms/_wwwroot/sitefiles/version.txt

FROM base AS final
WORKDIR /app
COPY --from=publish /app/sscms .
ENTRYPOINT ["dotnet", "SSCMS.Web.dll"]

# docker build -t sscms/core:dev .