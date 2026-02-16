# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY HrmsAiBot.csproj .
RUN dotnet restore HrmsAiBot.csproj

# Copy the entire project
COPY . .

# Build the project
RUN dotnet build HrmsAiBot.csproj -c Release -o /app/build

# Publish the project
RUN dotnet publish HrmsAiBot.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install wait-for-it dependencies
RUN apt-get update && apt-get install -y netcat-traditional && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Copy wait-for-it script
COPY wait-for-it.sh /usr/local/bin/wait-for-it.sh
RUN chmod +x /usr/local/bin/wait-for-it.sh

# Expose port for the application
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Run the application with wait for SQL Server
ENTRYPOINT ["/usr/local/bin/wait-for-it.sh", "sqlserver", "1433", "--", "dotnet", "HrmsAiBot.dll"]
