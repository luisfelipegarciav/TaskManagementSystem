FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything from the context (the solution root) into /app
COPY . .

# Restore using the solution file
RUN dotnet restore ./TaskManagementSystem.sln

# Publish the WebApi project
RUN dotnet publish ./src/TaskManagementSystem.WebApi/TaskManagementSystem.WebApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "TaskManagementSystem.WebApi.dll"]