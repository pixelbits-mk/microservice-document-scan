# Use a Debian-based image as a base image for .NET 6.0
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Set ASPNETCORE_ENVIRONMENT to "Development"
ENV ASPNETCORE_ENVIRONMENT=Development

# Install ClamAV
RUN apt-get update && \
    apt-get install -y clamav clamav-daemon && \
    freshclam

# Copy ClamAV configuration and signature database
COPY ./clamad.conf /etc/clamav/clamd.conf
# COPY ./clamav/* /var/lib/clamav/

# Copy the project files and restore as distinct layers
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DocumentScanner.Api/DocumentScanner.Api.csproj", "DocumentScanner.Api/"]
RUN dotnet restore "DocumentScanner.Api/DocumentScanner.Api.csproj"

COPY . .
WORKDIR "/src/DocumentScanner.Api"
RUN dotnet build "DocumentScanner.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentScanner.Api.csproj" -c Release -o /app/publish

# Create the final image using the runtime image and the publish output
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ENTRYPOINT ["dotnet", "DocumentScanner.Api.dll"]
