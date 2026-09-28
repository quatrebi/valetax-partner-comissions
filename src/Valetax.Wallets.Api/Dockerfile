FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV Kestrel__Endpoints__Http__Url=http://0.0.0.0:8080 \
    Kestrel__Endpoints__Grpc__Url=http://0.0.0.0:8081
ENV OTEL_SERVICE_NAME=wallets-api \
    OTEL_EXPORTER_OTLP_PROTOCOL=grpc
EXPOSE 8080 8081
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
COPY src/Valetax.Wallets.Api.Grpc/Valetax.Wallets.Api.Grpc.csproj src/Valetax.Wallets.Api.Grpc/
COPY src/Valetax.Wallets.Api.Grpc/Protos/ src/Valetax.Wallets.Api.Grpc/Protos/
COPY src/Valetax.Partners.Api.Contracts/Valetax.Partners.Api.Contracts.csproj src/Valetax.Partners.Api.Contracts/
COPY src/Valetax.Wallets.Api/Valetax.Wallets.Api.csproj src/Valetax.Wallets.Api/
RUN dotnet restore src/Valetax.Wallets.Api/Valetax.Wallets.Api.csproj
COPY . .
RUN dotnet publish src/Valetax.Wallets.Api/Valetax.Wallets.Api.csproj \
    -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Valetax.Wallets.Api.dll"]
