FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
ENV OTEL_SERVICE_NAME=valetax-api \
    OTEL_EXPORTER_OTLP_PROTOCOL=grpc
EXPOSE 8080
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

FROM mcr.microsoft.com/dotnet/sdk:10.0.201 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY global.json ./
COPY src/Valetax.Infrastructure/Valetax.Infrastructure.csproj src/Valetax.Infrastructure/
COPY src/Valetax.Partners.Api.Grpc/Valetax.Partners.Api.Grpc.csproj src/Valetax.Partners.Api.Grpc/
COPY src/Valetax.Partners.Api.Grpc/Protos/ src/Valetax.Partners.Api.Grpc/Protos/
COPY src/Valetax.Wallets.Api.Grpc/Valetax.Wallets.Api.Grpc.csproj src/Valetax.Wallets.Api.Grpc/
COPY src/Valetax.Wallets.Api.Grpc/Protos/ src/Valetax.Wallets.Api.Grpc/Protos/
COPY src/Valetax.Api/Valetax.Api.csproj src/Valetax.Api/
RUN dotnet restore src/Valetax.Api/Valetax.Api.csproj
COPY . .
RUN dotnet publish src/Valetax.Api/Valetax.Api.csproj \
    -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Valetax.Api.dll"]
