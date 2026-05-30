#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:a6df1767e8363f13963cc5da5c24d0403b630f66acbf5fd6bc414269e446c8fb AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-noble@sha256:6b0b7f73dc7cce85fe9eaf7cfcfd1dc109accc5b3782c8cba006fbe036da424e AS build
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
RUN echo `date +%Y-%m-%d-%H-%M-%S` > /app/sscms/_wwwroot/sitefiles/version.txt

FROM base AS final
WORKDIR /app
COPY --from=publish --chown=1654:1654 /app/sscms .
USER 1654
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 CMD ["dotnet", "--list-runtimes"]
ENTRYPOINT ["dotnet", "SSCMS.Web.dll"]

# docker build -t sscms/core:dev .
