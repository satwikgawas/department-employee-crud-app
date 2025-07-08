# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . ./

# Add NuGet.config to avoid Visual Studio fallback error
COPY NuGet.config ./ 

RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose the default port
EXPOSE 80

ENTRYPOINT ["dotnet", "EmployeeDepartmentCRUDApp.dll"]
