# syntax=docker/dockerfile:1.7
# Targets:
#   final    (default) - the API image
#   migrator           - EF Core migration bundle, run once per deployment (never by every API replica)

ARG DOTNET_SDK_VERSION=10.0.301
ARG ASPNET_VERSION=10.0.12

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_SDK_VERSION} AS build
ARG APP_VERSION=1.0.0
WORKDIR /src

# Restore in its own layer so source edits do not invalidate the package cache.
COPY global.json Directory.Build.props Directory.Packages.props .editorconfig ./
COPY src/ObservabilityApi.Domain/ObservabilityApi.Domain.csproj src/ObservabilityApi.Domain/
COPY src/ObservabilityApi.Application/ObservabilityApi.Application.csproj src/ObservabilityApi.Application/
COPY src/ObservabilityApi.Infrastructure/ObservabilityApi.Infrastructure.csproj src/ObservabilityApi.Infrastructure/
COPY src/ObservabilityApi.Api/ObservabilityApi.Api.csproj src/ObservabilityApi.Api/
RUN dotnet restore src/ObservabilityApi.Api/ObservabilityApi.Api.csproj

COPY src/ src/
RUN dotnet publish src/ObservabilityApi.Api/ObservabilityApi.Api.csproj \
      --configuration Release --no-restore --output /app/publish \
      -p:UseAppHost=false -p:Version=${APP_VERSION}

FROM build AS migrator-build
COPY dotnet-tools.json ./
RUN dotnet tool restore
# Building the bundle only needs the EF model; the host still requires a connection string to start, so give it
# a credential-free placeholder. The real one is supplied when the bundle runs.
RUN ConnectionStrings__Database="Server=build-placeholder;Database=build-placeholder" \
    dotnet ef migrations bundle \
      --project src/ObservabilityApi.Infrastructure \
      --startup-project src/ObservabilityApi.Api \
      --configuration Release \
      --output /app/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:${ASPNET_VERSION} AS migrator
WORKDIR /app
COPY --from=migrator-build /app/efbundle ./efbundle
USER $APP_UID
# Reads ConnectionStrings__Database from the environment (or pass --connection "<cs>").
ENTRYPOINT ["./efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:${ASPNET_VERSION} AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
# Non-root user provided by the official .NET images.
USER $APP_UID
ENTRYPOINT ["dotnet", "ObservabilityApi.Api.dll"]
