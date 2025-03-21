# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY *.sln ./
COPY MicroserviceProject/*.csproj MicroserviceProject/
COPY InfrastructerLayers/PM.Infrastructer/*.csproj InfrastructerLayers/PM.Infrastructer/
COPY InfrastructerLayers/PM.Persistence/*.csproj InfrastructerLayers/PM.Persistence/

# Restore dependencies
RUN dotnet restore

# Copy the entire source code and build the app
COPY . .
WORKDIR /app/MicroserviceProject
RUN dotnet publish -c Release -o /publish --no-restore

# Stage 2: Create a smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

# Copy the compiled application from the build stage
COPY --from=build /publish .

# Set a non-root user for security
RUN addgroup appgroup && adduser -S appuser -G appgroup
USER appuser

# Expose port
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "MicroserviceProject.dll"]
