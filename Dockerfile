FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["MeuAcervoAPI.sln", "./"]
COPY ["src/MeuAcervo.API/MeuAcervo.API.csproj", "src/MeuAcervo.API/"]
COPY ["src/MeuAcervo.Application/MeuAcervo.Application.csproj", "src/MeuAcervo.Application/"]
COPY ["src/MeuAcervo.Domain/MeuAcervo.Domain.csproj", "src/MeuAcervo.Domain/"]
COPY ["src/MeuAcervo.Infrastructure/MeuAcervo.Infrastructure.csproj", "src/MeuAcervo.Infrastructure/"]
COPY ["src/MeuAcervo.Shared/MeuAcervo.Shared.csproj", "src/MeuAcervo.Shared/"]

RUN dotnet restore "MeuAcervoAPI.sln"

COPY . .
RUN dotnet publish "src/MeuAcervo.API/MeuAcervo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MeuAcervo.API.dll"]
