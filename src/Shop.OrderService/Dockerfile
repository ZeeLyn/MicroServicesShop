FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.OrderService/Shop.OrderService.csproj src/Shop.OrderService/
COPY src/Shop.IOrder/Shop.IOrder.csproj src/Shop.IOrder/
COPY src/Shop.Common/Shop.Common.csproj src/Shop.Common/
RUN dotnet restore src/Shop.IOrder/Shop.IOrder.csproj
COPY . .
WORKDIR /src/src/Shop.IOrder
RUN dotnet build Shop.IOrder.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.IOrder.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.OrderService.dll"]
