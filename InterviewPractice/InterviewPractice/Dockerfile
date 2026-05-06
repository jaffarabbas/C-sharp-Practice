FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InterviewPractice.csproj", "./"]
RUN dotnet restore "InterviewPractice.csproj"

COPY . .
RUN dotnet publish "InterviewPractice.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "InterviewPractice.dll"]
