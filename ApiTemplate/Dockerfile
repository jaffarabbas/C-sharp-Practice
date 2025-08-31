FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiTemplate/ApiTemplate.csproj", "ApiTemplate/"]
COPY ["DBLayer/DBLayer.csproj", "DBLayer/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "ApiTemplate/ApiTemplate.csproj"
COPY . .
WORKDIR "/src/ApiTemplate"
RUN dotnet build "ApiTemplate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiTemplate.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiTemplate.dll"]