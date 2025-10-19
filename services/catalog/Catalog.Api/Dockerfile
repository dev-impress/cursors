# build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Directory.Build.props ./
COPY services/catalog/Catalog.Domain/Catalog.Domain.csproj services/catalog/Catalog.Domain/
COPY services/catalog/Catalog.Application/Catalog.Application.csproj services/catalog/Catalog.Application/
COPY services/catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj services/catalog/Catalog.Infrastructure/
COPY services/catalog/Catalog.Api/Catalog.Api.csproj services/catalog/Catalog.Api/
RUN dotnet restore services/catalog/Catalog.Api/Catalog.Api.csproj
COPY . .
RUN dotnet publish services/catalog/Catalog.Api/Catalog.Api.csproj -c Release -o /app/publish

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
