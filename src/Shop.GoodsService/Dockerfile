FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app
EXPOSE 5002

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.GoodsService/Shop.GoodsService.csproj src/Shop.GoodsService/
COPY src/Shop.IGoods/Shop.IGoods.csproj src/Shop.IGoods/
COPY src/Shop.Common/Shop.Common.csproj src/Shop.Common/
RUN dotnet restore src/Shop.GoodsService/Shop.GoodsService.csproj
COPY . .
WORKDIR /src/src/Shop.GoodsService
RUN dotnet build Shop.GoodsService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.GoodsService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.GoodsService.dll"]
