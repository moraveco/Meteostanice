FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base 
WORKDIR /app
RUN mkdir /app/data /app/logs && chown $APP_UID /app/data /app/logs
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Meteostanice/Meteostanice.csproj", "Meteostanice/"]
RUN dotnet restore "Meteostanice/Meteostanice.csproj"
COPY . .
WORKDIR "/src/Meteostanice"
RUN dotnet build "./Meteostanice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Meteostanice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meteostanice.dll"]
