# --- Step 1: Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# --- Step 2: Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app from build
COPY --from=build /app/publish ./

# Expose port 80 (Render uses this)
EXPOSE 80

# Entry point
ENTRYPOINT ["dotnet", "EmployeeDepartmentCRUDApp.dll"]
