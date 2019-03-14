FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.Gateway.Basket/Shop.Gateway.Basket.csproj src/Shop.Gateway.Basket/
RUN dotnet restore src/Shop.Gateway.Basket/Shop.Gateway.Basket.csproj
COPY . .
WORKDIR /src/src/Shop.Gateway.Basket
RUN dotnet build Shop.Gateway.Basket.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.Gateway.Basket.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Gateway.Basket.dll"]
