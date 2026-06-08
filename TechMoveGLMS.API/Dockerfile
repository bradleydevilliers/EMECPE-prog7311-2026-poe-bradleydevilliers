# I used a multi‑stage build to keep the final image small: first restore + publish, then copy only the published output.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY . .

# Restore and publish API
RUN dotnet restore "TechMoveGLMS.API/TechMoveGLMS.API.csproj"
RUN dotnet publish "TechMoveGLMS.API/TechMoveGLMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.API.dll"]



# Docker, 2026. Multi-stage build for .NET applications.[Online] 
# Available at: https://docs.docker.com/language/dotnet/