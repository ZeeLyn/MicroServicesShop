FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY src/Shop.Identity/Shop.Identity.csproj src/Shop.Identity/
RUN dotnet restore src/Shop.Identity/Shop.Identity.csproj
COPY . .
WORKDIR /src/src/Shop.Identity
RUN dotnet build Shop.Identity.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Shop.Identity.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Identity.dll"]
