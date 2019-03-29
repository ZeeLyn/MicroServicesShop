FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.Gateway.Order/Shop.Gateway.Order.csproj src/Shop.Gateway.Order/
COPY src/Shop.IOrder/Shop.IOrder.csproj src/Shop.IOrder/
COPY src/Shop.Common/Shop.Common.csproj src/Shop.Common/
RUN dotnet restore src/Shop.Gateway.Order/Shop.Gateway.Order.csproj
COPY . .
WORKDIR /src/src/Shop.Gateway.Order
RUN dotnet build Shop.Gateway.Order.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.Gateway.Order.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Gateway.Order.dll"]
