# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG NUGET_PAT
RUN nuget sources Add -Name "kangmike" -Source "https://pkgs.dev.azure.com/kangmike/_packaging/kangmike/nuget/v3/index.json" -UserName kangmike -Password qrk2odzaf75zr4vckj2b5xlv2mkmvyc27t2wpt3du3no3ol7sqmq


COPY ["AvScanner.Api/AvScanner.Api.csproj", "AvScanner.Api/"]
RUN dotnet restore "AvScanner.Api/AvScanner.Api.csproj"

COPY . .
WORKDIR "/src/AvScanner.Api"
RUN dotnet build "AvScanner.Api.csproj" -c Release -o /app/build
RUN dotnet publish "AvScanner.Api.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
EXPOSE 80


# Set ASPNETCORE_ENVIRONMENT to "Development"
ENV ASPNETCORE_ENVIRONMENT=Development

# Install ClamAV
RUN apt-get update && \
    apt-get install -y clamav clamav-daemon && \
    freshclam

# Copy the ClamAV configuration file 
COPY ./clamd.conf /etc/clamav/clamd.conf

# Remove carriage returns from the ClamAV configuration file
RUN tr -d '\r' < /etc/clamav/clamd.conf > /etc/clamav/clamd.conf.tmp && \
    mv /etc/clamav/clamd.conf.tmp /etc/clamav/clamd.conf

# Set permissions on the ClamAV configuration file
RUN chmod 644 /etc/clamav/clamd.conf

# Copy published files from build stage
COPY --from=build /app/publish .

ENTRYPOINT ["sh", "-c", "service clamav-daemon start && exec dotnet AvScanner.Api.dll"]
