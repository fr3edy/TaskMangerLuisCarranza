# Etapa 1: Imagen base ligera para producción
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Etapa 2: Imagen pesada con el SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos las referencias de todos los proyectos para restaurar dependencias
COPY ["TaskManager.Api/TaskManager.Api.csproj", "TaskManager.Api/"]
COPY ["TaskManager.Application/TaskManager.Application.csproj", "TaskManager.Application/"]
COPY ["TaskManager.Domain/TaskManager.Domain.csproj", "TaskManager.Domain/"]
COPY ["TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "TaskManager.Infrastructure/"]
RUN dotnet restore "./TaskManager.Api/TaskManager.Api.csproj"

# Copiamos el resto del código y compilamos
COPY . .
WORKDIR "/src/TaskManager.Api"
RUN dotnet build "./TaskManager.Api.csproj" -c Release -o /app/build

# Etapa 3: Publicamos los binarios optimizados
FROM build AS publish
RUN dotnet publish "./TaskManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 4: Imagen final que ejecutará la aplicación
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]