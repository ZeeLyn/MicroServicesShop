FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/Shop.Gateway.Goods/Shop.Gateway.Goods.csproj", "src/Shop.Gateway.Goods/"]
COPY ["src/Shop.IGoods/Shop.IGoods.csproj", "src/Shop.IGoods/"]
COPY ["src/Shop.Common/Shop.Common.csproj", "src/Shop.Common/"]
RUN dotnet restore "src/Shop.Gateway.Goods/Shop.Gateway.Goods.csproj"
COPY . .
WORKDIR "/src/src/Shop.Gateway.Goods"
RUN dotnet build "Shop.Gateway.Goods.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Shop.Gateway.Goods.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Gateway.Goods.dll"]