# syntax=docker/dockerfile:1.9

#############################
#  Base (runtime)
#############################
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

# Globalization for pt-BR
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs

#############################
#  Build
#############################
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy csprojs for restore cache
COPY Api/*.csproj Api/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/
COPY CrossCutting/*.csproj CrossCutting/
COPY *.sln ./

# Cached restore
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore Api/Api.csproj

# Copy all source
COPY . .

# Publish optimized for container
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish Api/Api.csproj -c Release -o /app/publish \
      /p:UseAppHost=false /p:PublishSingleFile=false /p:DebugType=None

#############################
#  Final (runtime image)
#############################
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
