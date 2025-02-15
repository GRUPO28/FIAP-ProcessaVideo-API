# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Definir o diretório de trabalho
WORKDIR /src

# Copiar o arquivo .csproj e restaurar as dependências
COPY ["src/FIAP-ProcessaVideo-API/FIAP-ProcessaVideo-API.csproj", "src/FIAP-ProcessaVideo-API/"]
RUN dotnet restore "src/FIAP-ProcessaVideo-API/FIAP-ProcessaVideo-API.csproj"

# Copiar o restante do código
COPY . .

# Definir o diretório de trabalho para o build
WORKDIR "/src/src/FIAP-ProcessaVideo-API"
RUN dotnet build "FIAP-ProcessaVideo-API.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "FIAP-ProcessaVideo-API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final com imagem leve
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

# Expor a porta desejada
ENV ASPNETCORE_HTTP_PORTS=5158
EXPOSE 5158

# Definir o diretório de trabalho
WORKDIR /app

# Copiar a versão publicada para a imagem final
COPY --from=publish /app/publish .

# Definir o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "FIAP-ProcessaVideo-API.dll"]

