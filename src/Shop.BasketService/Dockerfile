FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.BasketService/Shop.BasketService.csproj src/Shop.BasketService/
RUN dotnet restore src/Shop.BasketService/Shop.BasketService.csproj
COPY . .
WORKDIR /src/src/Shop.BasketService
RUN dotnet build Shop.BasketService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.BasketService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.BasketService.dll"]
