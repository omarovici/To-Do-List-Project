# ======== Stage 1: Build ========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore as distinct layers
COPY "To-Do-List Project.sln" .
COPY "To-Do-List Project/To-Do-List Project.csproj" "./To-Do-List Project/"
RUN dotnet restore "./To-Do-List Project/To-Do-List Project.csproj"

# Copy everything else and build
COPY . ./
WORKDIR "/app/To-Do-List Project"
RUN dotnet publish -c Release -o out

# ======== Stage 2: Runtime ========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Set non-root user for security
RUN useradd -m appuser
USER appuser

# Copy published files from build stage
COPY --from=build /app/out ./

# Set environment variable if needed
ENV ASPNETCORE_URLS=http://+:5000

# Expose port
EXPOSE 5000

# Run app
ENTRYPOINT ["dotnet", "To-Do-List Project.dll"]
