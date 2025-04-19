# Use the official ASP.NET 8.0 runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["To-Do-List.csproj", "."]
RUN dotnet restore "To-Do-List.csproj"

# Copy the rest of the application source code
COPY . .
WORKDIR "/src"

# Build the application in Release mode
RUN dotnet build "To-Do-List.csproj" -c Release -o /app/build

# Publish the application to a folder for deployment
FROM build AS publish
RUN dotnet publish "To-Do-List.csproj" -c Release -o /app/publish

# Final stage: build a runtime image
FROM base AS final
WORKDIR /app

# Copy the published output to the runtime image
COPY --from=publish /app/publish .

# Set the entrypoint to run the application
ENTRYPOINT ["dotnet", "To-Do-List.dll"]
