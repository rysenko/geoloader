# Use the official Mono image as the base image
FROM mono:latest

# Install dependencies (if any additional dependencies are required, add them here)
RUN apt-get update && apt-get install -y \
    nunit-console && \
    rm -rf /var/lib/apt/lists/*

# Set the working directory in the container
WORKDIR /app

# Copy the entire workspace into the container
COPY . /app

# Build the solution and publish as a single file for Windows
RUN msbuild GeoLoader.sln /p:Configuration=Release /p:PublishSingleFile=true /p:RuntimeIdentifier=win-x64 /p:PublishTrimmed=true

# After building the solution, copy the single executable to the root directory
RUN cp GeoLoader/bin/Release/GeoLoader.exe /app/GeoLoader.exe