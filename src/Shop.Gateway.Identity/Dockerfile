FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.Gateway.Identity/Shop.Gateway.Identity.csproj src/Shop.Gateway.Identity/
COPY src/Shop.IIdentity/Shop.IIdentity.csproj src/Shop.IIdentity/
COPY src/Shop.Common/Shop.Common.csproj src/Shop.Common/
RUN dotnet restore src/Shop.Gateway.Identity/Shop.Gateway.Identity.csproj
COPY . .
WORKDIR /src/src/Shop.Gateway.Identity
RUN dotnet build Shop.Gateway.Identity.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.Gateway.Identity.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Gateway.Identity.dll"]
