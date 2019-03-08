FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.IdentityService/Shop.IdentityService.csproj src/Shop.IdentityService/
COPY src/Shop.IIdentity/Shop.IIdentity.csproj src/Shop.IIdentity/
COPY src/Shop.Common/Shop.Common.csproj src/Shop.Common/
RUN dotnet restore src/Shop.IdentityService/Shop.IdentityService.csproj
COPY . .
WORKDIR /src/src/Shop.IdentityService
RUN dotnet build Shop.IdentityService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.IdentityService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.IdentityService.dll"]
