# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 9229  


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiTemplate/ApiTemplate.csproj", "ApiTemplate/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["DBLayer/DBLayer.csproj", "DBLayer/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "./ApiTemplate/ApiTemplate.csproj"
COPY . .
WORKDIR "/src/ApiTemplate"
RUN dotnet build "./ApiTemplate.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiTemplate.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080;http://+:8081

ENTRYPOINT ["dotnet", "ApiTemplate.dll"]