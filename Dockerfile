# ========================
# 1) Build stage
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution trước để cache
COPY ["Lingrow.sln", "./"]

# Copy csproj để cache restore
COPY ["Lingrow.Api/Lingrow.Api.csproj", "Lingrow.Api/"]
COPY ["Lingrow.BusinessLogicLayer/Lingrow.BusinessLogicLayer.csproj", "Lingrow.BusinessLogicLayer/"]
COPY ["Lingrow.DataAccessLayer/Lingrow.DataAccessLayer.csproj", "Lingrow.DataAccessLayer/"]
COPY ["Lingrow.Object/Lingrow.Object.csproj", "Lingrow.Object/"]

# Restore
RUN dotnet restore

# Copy source & publish
COPY . .
RUN dotnet publish Lingrow.Api/Lingrow.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================
# 2) Runtime stage
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Không đặt ASPNETCORE_URLS ở đây; để compose truyền (5000)
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Lingrow.Api.dll"]
