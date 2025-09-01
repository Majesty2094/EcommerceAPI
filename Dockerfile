# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore


# Copy everything (including Images folder)
COPY . .

# OR explicitly copy Images if you want to be sure
COPY Images ./Images


# Build and publish the app
RUN dotnet publish -c Release -o /app

# Use the ASP.NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published files from the build container
COPY --from=build /app .

# Expose port 8080 (Render expects apps to listen on $PORT)
EXPOSE 8080

# Set the entrypoint
ENTRYPOINT ["dotnet", "MajesticEcommerceAPI.dll"]
