FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LegoLibrary.slnx .
COPY src/LegoTracker/LegoTracker.csproj src/LegoTracker/
RUN dotnet restore src/LegoTracker/LegoTracker.csproj

COPY src/LegoTracker/ src/LegoTracker/
RUN dotnet publish src/LegoTracker/LegoTracker.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/data /app/media

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/app/data/legotracker.db"
ENV Media__RootPath=/app/media
EXPOSE 8080

ENTRYPOINT ["dotnet", "LegoTracker.dll"]
