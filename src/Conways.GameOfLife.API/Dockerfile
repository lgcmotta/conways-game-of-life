FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Conways.GameOfLife.API/Conways.GameOfLife.API.csproj", "src/Conways.GameOfLife.API/"]
COPY ["src/Conways.GameOfLife.Infrastructure.PostgresSQL/Conways.GameOfLife.Infrastructure.PostgresSQL.csproj", "src/Conways.GameOfLife.Infrastructure.PostgresSQL/"]
COPY ["src/Conways.GameOfLife.Domain/Conways.GameOfLife.Domain.csproj", "src/Conways.GameOfLife.Domain/"]
COPY ["src/Conways.GameOfLife.Infrastructure/Conways.GameOfLife.Infrastructure.csproj", "src/Conways.GameOfLife.Infrastructure/"]
RUN dotnet restore "src/Conways.GameOfLife.API/Conways.GameOfLife.API.csproj"
COPY . .
WORKDIR "/src/src/Conways.GameOfLife.API"
RUN dotnet build "Conways.GameOfLife.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Conways.GameOfLife.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Conways.GameOfLife.API.dll"]
