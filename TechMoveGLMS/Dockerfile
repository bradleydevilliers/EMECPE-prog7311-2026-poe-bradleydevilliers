# I followed the same multi‑stage pattern for the MVC app. The container exposes port 8080 internally.
# Docker, 2026. Containerising ASP.NET Core MVC applications.[Online] 
# Available at: https://docs.docker.com/samples/dotnetcore/
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY . .

# Restore and publish MVC
RUN dotnet restore "TechMoveGLMS/TechMoveGLMS.csproj"
RUN dotnet publish "TechMoveGLMS/TechMoveGLMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechMoveGLMS.dll"]


# Docker, 2026. Containerising ASP.NET Core MVC applications.[Online] 
# Available at: https://docs.docker.com/samples/dotnetcore/