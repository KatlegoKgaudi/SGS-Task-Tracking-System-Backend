# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/API/SGS.TaskTracker.API.csproj", "src/API/"]
COPY ["src/Application/SGS.TaskTracker.Application.csproj", "src/Application/"]
COPY ["src/Core/SGS.TaskTracker.Core.csproj", "src/Core/"]
COPY ["src/Infrastructure/SGS.TaskTracker.Infrastructure.csproj", "src/Infrastructure/"]

#COPY ["tests/SGS.TaskTracker.Tests.csproj", "tests/"]

# Restore packages
RUN dotnet restore "src/API/SGS.TaskTracker.API.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/API"
RUN dotnet build "SGS.TaskTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SGS.TaskTracker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
RUN mkdir -p /app/keys
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SGS.TaskTracker.API.dll"]