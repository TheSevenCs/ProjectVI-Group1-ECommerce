# See https://aka.ms/customizecontainer for more details on Visual Studio Docker debugging

# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ Fix: Corrected Path to `.csproj`
COPY ["EcommerceWebApp/EcommerceWebApp/EcommerceWebApp.csproj", "EcommerceWebApp/"]

RUN dotnet restore "EcommerceWebApp/EcommerceWebApp.csproj"
COPY . .
WORKDIR "/src/EcommerceWebApp/EcommerceWebApp"

# ✅ Fix: Ensure correct path to `.csproj`
RUN dotnet build "EcommerceWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcommerceWebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcommerceWebApp.dll"]
