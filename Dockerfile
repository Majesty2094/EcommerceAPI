# Use the official ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Render provides a PORT environment variable
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MajesticEcommerceAPI/MajesticEcommerceAPI.csproj", "MajesticEcommerceAPI/"]
RUN dotnet restore "./MajesticEcommerceAPI/MajesticEcommerceAPI.csproj"
COPY . .
WORKDIR "/src/MajesticEcommerceAPI"
RUN dotnet build "./MajesticEcommerceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MajesticEcommerceAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MajesticEcommerceAPI.dll"]
