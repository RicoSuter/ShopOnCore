FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ShopOnCore.Orders.MessageProcessor.App/ShopOnCore.Orders.MessageProcessor.App.csproj ShopOnCore.Orders.MessageProcessor.App/
RUN dotnet restore ShopOnCore.Orders.MessageProcessor.App/ShopOnCore.Orders.MessageProcessor.App.csproj
COPY . .
WORKDIR /src/ShopOnCore.Orders.MessageProcessor.App
RUN dotnet build ShopOnCore.Orders.MessageProcessor.App.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ShopOnCore.Orders.MessageProcessor.App.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShopOnCore.Orders.MessageProcessor.App.dll"]
