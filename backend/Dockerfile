# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Сначала копируем только csproj для кэширования слоя restore.
COPY src/ResalePlatform.Domain/*.csproj src/ResalePlatform.Domain/
COPY src/ResalePlatform.Application/*.csproj src/ResalePlatform.Application/
COPY src/ResalePlatform.Infrastructure/*.csproj src/ResalePlatform.Infrastructure/
COPY src/ResalePlatform.API/*.csproj src/ResalePlatform.API/
RUN dotnet restore src/ResalePlatform.API/ResalePlatform.API.csproj

# Копируем исходники и публикуем.
COPY src/ src/
RUN dotnet publish src/ResalePlatform.API/ResalePlatform.API.csproj -c Release -o /app --no-restore

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "ResalePlatform.API.dll"]
