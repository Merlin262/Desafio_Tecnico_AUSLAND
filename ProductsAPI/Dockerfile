# Estágio 1 — build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copia os csproj e restaura dependências (camada cacheável)
COPY ["ProductsAPI/ProductsAPI.API.csproj", "ProductsAPI/"]
COPY ["ProductsAPI.Application/ProductsAPI.Application.csproj", "ProductsAPI.Application/"]
COPY ["ProductsAPI.Domain/ProductsAPI.Domain.csproj", "ProductsAPI.Domain/"]
COPY ["ProductsAPI.Infrastructure/ProductsAPI.Infrastructure.csproj", "ProductsAPI.Infrastructure/"]

RUN dotnet restore "ProductsAPI/ProductsAPI.API.csproj"

# copia o restante e publica
COPY . .
WORKDIR "/src/ProductsAPI"
RUN dotnet publish -c Release -o /app/publish

# Estágio 2 — runtime (imagem final menor)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# roda migrations automaticamente ao iniciar
ENTRYPOINT ["dotnet", "ProductsAPI.API.dll"]