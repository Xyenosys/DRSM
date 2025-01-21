# Base image for ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y sudo curl lib32gcc-s1 procps net-tools

# Create the steam user and add to sudoers
RUN groupadd -g 1000 steam && \
    useradd -m -s /bin/bash -u 1000 -g 1000 steam && \
    echo "steam ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers && \
    mkdir -p /home/steam/Steam && \
    chown -R steam:steam /home/steam

WORKDIR /app

# Build stage for compiling and publishing the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DRSM/DRSM.csproj", "DRSM/"]
RUN dotnet restore "DRSM/DRSM.csproj"
COPY . .
WORKDIR "/src/DRSM"
RUN dotnet publish "DRSM.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final production stage
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Switch to steam user and run application
USER steam
ENTRYPOINT ["dotnet", "DRSM.dll"]