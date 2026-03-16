# ─────────────────────────────────────────
# Stage 1: Build the .NET application
# Build context is the repo root (set in docker-compose.yml)
# ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for layer caching
COPY Directory.Build.props ./
COPY src/EngagementTracker.Api/EngagementTracker.Api.csproj src/EngagementTracker.Api/
COPY src/EngagementTracker.Core/EngagementTracker.Core.csproj src/EngagementTracker.Core/
COPY src/EngagementTracker.Infrastructure/EngagementTracker.Infrastructure.csproj src/EngagementTracker.Infrastructure/

# Restore dependencies (cached unless .csproj files change)
RUN dotnet restore src/EngagementTracker.Api/EngagementTracker.Api.csproj

# Copy all source code and publish
COPY src/EngagementTracker.Api/ src/EngagementTracker.Api/
COPY src/EngagementTracker.Core/ src/EngagementTracker.Core/
COPY src/EngagementTracker.Infrastructure/ src/EngagementTracker.Infrastructure/
RUN dotnet publish src/EngagementTracker.Api/EngagementTracker.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─────────────────────────────────────────
# Stage 2: Runtime image (minimal footprint)
# ─────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Security: run as non-root user
RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EngagementTracker.Api.dll"]
