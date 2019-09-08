FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["CryptoData/CryptoData.csproj", "CryptoData/"]
RUN dotnet restore "CryptoData/CryptoData.csproj"
COPY . .
WORKDIR "/src/CryptoData"
RUN dotnet build "CryptoData.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "CryptoData.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CryptoData.dll"]