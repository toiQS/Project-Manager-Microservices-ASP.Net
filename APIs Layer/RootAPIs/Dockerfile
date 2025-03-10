# Stage 1:  Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
#restore
COPY ["APIs Layer/RootAPIs/RootAPIs.csproj", "APIs Layer/RootAPIs/"]
RUN dotnet restore "APIs Layer/RootAPIs/RootAPIs.csproj"
#build
COPY . .
WORKDIR "/app/APIs Layer/RootAPIs"
# RUN dotnet build "APIs Layer/RootAPIs/RootAPIs.csproj" -c Release -o /app/build
RUN ls -R /app

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "APIs Layer/RootAPIs/RootAPIs.csproj" -c Release -o /app/publish

# Stage 3: Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
ENV ASPNETCORE_HTTPS_PORT=7200
EXPOSE 7200
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RootAPIs.dll"]