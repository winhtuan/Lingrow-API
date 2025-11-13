# ========================
# 1) Build stage
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution trước để cache
COPY ["Plantpedia.sln", "./"]

# Copy csproj để cache restore
COPY ["Plantpedia.Api/Plantpedia.Api.csproj", "Plantpedia.Api/"]
COPY ["Plantpedia.BusinessLogicLayer/Plantpedia.BusinessLogicLayer.csproj", "Plantpedia.BusinessLogicLayer/"]
COPY ["Plantpedia.DataAccessLayer/Plantpedia.DataAccessLayer.csproj", "Plantpedia.DataAccessLayer/"]
COPY ["Plantpedia.Object/Plantpedia.Object.csproj", "Plantpedia.Object/"]

# Restore
RUN dotnet restore

# Copy source & publish
COPY . .
RUN dotnet publish Plantpedia.Api/Plantpedia.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================
# 2) Runtime stage
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Không đặt ASPNETCORE_URLS ở đây; để compose truyền (5000)
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Plantpedia.Api.dll"]
