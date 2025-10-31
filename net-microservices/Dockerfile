# ============================================
# ETAPA 1: BUILD
# ============================================
# Usamos la imagen del SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias
# Esto se hace primero para aprovechar la caché de Docker
COPY ["webapi.csproj", "./"]
RUN dotnet restore "webapi.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la aplicación en modo Release
RUN dotnet build "webapi.csproj" -c Release -o /app/build

# ============================================
# ETAPA 2: PUBLISH
# ============================================
# Publicar la aplicación optimizada
FROM build AS publish
RUN dotnet publish "webapi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================
# ETAPA 3: RUNTIME (Imagen final)
# ============================================
# Usamos la imagen ligera de runtime (solo para ejecutar)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la etapa anterior
COPY --from=publish /app/publish .

# Exponer el puerto 8080 (puerto por defecto en .NET 8)
EXPOSE 8080

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "webapi.dll"]