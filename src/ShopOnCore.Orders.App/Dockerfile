FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
COPY . .
WORKDIR /ShopOnCore.Orders.App
RUN dotnet build ShopOnCore.Orders.App.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ShopOnCore.Orders.App.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShopOnCore.Orders.App.dll"]
